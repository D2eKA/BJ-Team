using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    #region ������� � ��������� ��������
    
    // ������� ��� ���������� � ��������� ����� � �������
    public event Action OnQueueSpaceAvailable;
    
    // �������� ������������� �������
    public bool IsQueueFull => customerQueue.Count >= queuePositions.Count;
    
    #endregion
    
    #region ������������� ����
    
    [Header("��������� ������������ ������")]
    [SerializeField] private bool infiniteModeEnabled = true;
    [SerializeField] private float delayBetweenWaves = 10f;
    [SerializeField] private float difficultyMultiplier = 0.15f;
    
    [Header("��������� ��������")]
    [SerializeField] private float baseCustomerSpeed = 2f;
    [SerializeField] private int baseMaxRequestQuantity = 3;
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private GameObject satisfactionBarPrefab;
    
    [Header("������� � �����")]
    [SerializeField] private List<Transform> queuePositions = new List<Transform>();
    [SerializeField] private Transform table;
    [SerializeField] private Transform exitDoor;
    [SerializeField] private Transform entranceDoor;
    
    #endregion
    
    #region ��������� ����
    
    private Queue<Customer> customerQueue = new Queue<Customer>();
    private Customer currentCustomer;
    private bool isTableOccupied = false;
    private int currentWave = 1;
    private Transform customersContainer;
    
    #endregion
    
    #region ������ ���������� ����� Unity
    
    private void Awake()
    {
        // ������� ��� ������� ��������� ��� ��������
        customersContainer = GameObject.Find("Customers")?.transform;
        if (customersContainer == null)
        {
            GameObject container = new GameObject("Customers");
            customersContainer = container.transform;
        }
    }
    
    private void Start()
    {
        if (infiniteModeEnabled)
        {
            GenerateNextWave();
        }
    }
    
    private void Update()
    {
        CheckQueueStatus();
    }
    
    #endregion
    
    #region ��������� ������
    
    /// <summary>
    /// ��������� ������� � �������
    /// </summary>
    public void AddCustomerToQueue(Customer customer)
    {
        if (IsQueueFull)
        {
            Debug.LogWarning("������� ���������! ������ ������.");
            SendCustomerToExit(customer);
            return;
        }
        
        customerQueue.Enqueue(customer);
        UpdateQueuePositions();
        
        // ���� ���� �������� � ��� ������ ������, ���������� ��� � �����
        if (!isTableOccupied && customerQueue.Count == 1)
        {
            MoveCustomerToTable();
        }
    }
    
    /// <summary>
    /// ��������� �������������� ����� � ��������
    /// </summary>
    public void HandleHeroInteraction()
    {
        if (currentCustomer == null || !currentCustomer.IsReadyForInteraction())
            return;
        
        Debug.Log("����� ��������������� � ��������.");
        
        // �������� ������ ����������
        currentCustomer.HideRequest();
        
        // ������������� ���������� �����������������
        currentCustomer.StopSatisfactionDecrease();
        
        // ��������� ������ �� ��������� �������
        Customer customerToExit = currentCustomer;
        
        // ����������� ����
        currentCustomer.SetReadyForInteraction(false);
        isTableOccupied = false;
        currentCustomer = null;
        
        // ���������� ���������� ������� � �����
        MoveCustomerToTable();
        
        // ���������� ����������� ������� � ������
        SendCustomerToExit(customerToExit);
        
        // ���������� � ��������� ����� � �������
        OnQueueSpaceAvailable?.Invoke();
        
        // ��������� ������ �������
        CheckQueueStatus();
    }
    
    /// <summary>
    /// ��������� ����� ������� �� ��������� ��������
    /// </summary>
    public void HandleCustomerLeave(Customer leavingCustomer)
    {
        if (leavingCustomer != currentCustomer)
            return;
        
        // �������� ������ ����������
        leavingCustomer.HideRequest();
        
        // ����������� ����
        isTableOccupied = false;
        currentCustomer = null;
        
        // ���������� ������� � ������
        SendCustomerToExit(leavingCustomer);
        
        // ���������� ���������� ������� � �����
        MoveCustomerToTable();
        
        // ���������� � ��������� ����� � �������
        OnQueueSpaceAvailable?.Invoke();
    }
    
    /// <summary>
    /// ��������� �������� ������� � �����
    /// </summary>
    public Customer GetCurrentCustomer()
    {
        return currentCustomer;
    }
    
    #endregion
    
    #region ��������� ������
    
    /// <summary>
    /// �������� ��������� ������� ��� ������� ����� �����
    /// </summary>
    private void CheckQueueStatus()
    {
        if (infiniteModeEnabled && customerQueue.Count == 0 && currentCustomer == null)
        {
            if (!IsInvoking("GenerateNextWaveAfterDelay"))
            {
                Debug.Log($"������� �����������. ��������� ����� ����� {delayBetweenWaves} ������...");
                Invoke("GenerateNextWaveAfterDelay", delayBetweenWaves);
            }
        }
    }
    
    /// <summary>
    /// ������ ��������� ��������� ����� ����� ��������
    /// </summary>
    private void GenerateNextWaveAfterDelay()
    {
        currentWave++;
        Debug.Log($"�������� ��������� ����� {currentWave}");
        GenerateNextWave();
    }
    
    /// <summary>
    /// ��������� ����� ����� ��������
    /// </summary>
    private void GenerateNextWave()
    {
        try
        {
            float difficulty = 1f + (currentWave - 1) * difficultyMultiplier;
            
            // ���������� ���������� ��������
            int customersCount = Mathf.FloorToInt(5 * difficulty);
            customersCount = Mathf.Clamp(customersCount, 1, queuePositions.Count);
            
            // ���������� ������������ ���������� ������� � �������
            int maxRequestQuantity = Mathf.FloorToInt(baseMaxRequestQuantity * difficulty);
            maxRequestQuantity = Mathf.Clamp(maxRequestQuantity, 1, 10);
            
            // ���������� �������� ��������
            float customerSpeed = baseCustomerSpeed * (1f + (currentWave - 1) * 0.1f);
            
            Debug.Log($"����� {currentWave}: �������� {customersCount}, ������������ ���������� ������� {maxRequestQuantity}, �������� {customerSpeed:F1}");
            
            // ������� �������� � ��������� ���������
            StartCoroutine(SpawnCustomersWithDelay(customersCount, customerSpeed, maxRequestQuantity));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ ��� ��������� �����: {e.Message}");
        }
    }
    
    /// <summary>
    /// �������� �������� � ���������
    /// </summary>
    private IEnumerator SpawnCustomersWithDelay(int count, float speed, int maxRequestQuantity)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnCustomer(speed, maxRequestQuantity);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    /// <summary>
    /// �������� ������ �������
    /// </summary>
    private void SpawnCustomer(float speed, int maxRequestQuantity)
    {
        if (customerPrefab == null || entranceDoor == null)
        {
            Debug.LogError("customerPrefab ��� entranceDoor �� �������!");
            return;
        }
        
        try
        {
            // ������� ������� � �����
            GameObject customerObj = Instantiate(customerPrefab, entranceDoor.position, Quaternion.identity, customersContainer);
            Customer customer = customerObj.GetComponent<Customer>();
            
            if (customer != null)
            {
                // ����������� ��������� �������
                customer.moveSpeed = speed;
                
                // ������� � ��������� SatisfactionBar
                if (satisfactionBarPrefab != null)
                {
                    GameObject barObj = Instantiate(satisfactionBarPrefab, customersContainer);
                    SatisfactionBar bar = barObj.GetComponent<SatisfactionBar>();
                    
                    if (bar != null)
                    {
                        customer.SetSatisfactionBar(bar);
                    }
                }
                
                // ��������� ��������� ������
                AssignRandomRequest(customer, maxRequestQuantity);
                
                // ��������� ������� � �������
                AddCustomerToQueue(customer);
                
                Debug.Log($"������ ������ � �������� � �������");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ ��� �������� �������: {e.Message}");
        }
    }
    
    /// <summary>
    /// ���������� ���������� ������� �������
    /// </summary>
    private void AssignRandomRequest(Customer customer, int maxRequestQuantity)
    {
        // ���������� ��������� ���������� ������
        int randomQuantity = UnityEngine.Random.Range(1, maxRequestQuantity + 1);
        customer.RequestedQuantity = randomQuantity;
        
        // ���������� ProductManager ��� ��������� ��������� �������
        if (ProductManager.Instance != null)
        {
            List<Item.ItemType> availableTypes = ProductManager.Instance.GetAllProductTypes();
            availableTypes.RemoveAll(type => type == Item.ItemType.None);
            
            if (availableTypes.Count > 0)
            {
                // �������� ��������� �����
                int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);
                customer.RequestedItem = availableTypes[randomIndex];
                Debug.Log($"������� �������� �����: {customer.RequestedItem}, ����������: {customer.RequestedQuantity}");
                return;
            }
        }
        
        // �������� �������, ���� ProductManager ����������
        Item.ItemType[] types = { Item.ItemType.Potato, Item.ItemType.Cucumber, Item.ItemType.Tomato, Item.ItemType.Corn, Item.ItemType.Eggplant };
        int index = UnityEngine.Random.Range(0, types.Length);
        customer.RequestedItem = types[index];
    }
    
    /// <summary>
    /// ����������� ������� � �����
    /// </summary>
    private void MoveCustomerToTable()
    {
        if (customerQueue.Count == 0) return;
        
        currentCustomer = customerQueue.Dequeue();
        isTableOccupied = true;
        
        // ���������� ������� � �����
        currentCustomer.MoveTo(table.position, () =>
        {
            Debug.Log("������ ���� �������������� � ������.");
            
            // ���������� ������ ����������
            currentCustomer.ShowRequest();
            
            // ��������� ���������� �����������������
            currentCustomer.StartSatisfactionDecrease();
        });
        
        currentCustomer.SetReadyForInteraction(true);
        UpdateQueuePositions();
    }
    
    /// <summary>
    /// ���������� ������� �������� � �������
    /// </summary>
    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (Customer customer in customerQueue)
        {
            if (index < queuePositions.Count)
            {
                customer.MoveTo(queuePositions[index].position);
                index++;
            }
        }
    }
    
    /// <summary>
    /// �������� ������� � ������
    /// </summary>
    private void SendCustomerToExit(Customer customer)
    {
        if (customer == null) return;
        
        customer.MoveTo(exitDoor.position, () =>
        {
            // ���������� SatisfactionBar ������ � ��������
            if (customer.GetSatisfactionBar() != null)
            {
                Destroy(customer.GetSatisfactionBar().gameObject);
            }
            
            Destroy(customer.gameObject);
        });
    }
    
    #endregion
}
