using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // ������� � �������
    [SerializeField] private Transform table; // ����
    [SerializeField] private Transform exitDoor; // �������� �����
    private SatisfactionBar satisfactionBar; //����� �������

    private Queue<Customer> customerQueue = new Queue<Customer>(); // ������� ������
    private Customer currentCustomer; // ����� � �����
    public bool isTableOccupied = false; // ����� �� ����

    public event Action OnQueueSpaceAvailable; // ������� ��� ����������� � ����������� �����\

    public void Update()
    {
        if(GameObject.Find("Customers").transform.childCount > 1)
        {
            if (GameObject.Find("Customers").transform.GetChild(1).GetComponent<SatisfactionBar>())
            {
                satisfactionBar = GameObject.Find("Customers").transform.GetChild(1).GetComponent<SatisfactionBar>();
            }
        }
    }

    public void AddCustomerToQueue(Customer customer)
    {
        if (customerQueue.Count < queuePositions.Count)
        {
            customerQueue.Enqueue(customer);
            UpdateQueuePositions();

            // ���� ���� ��������, ���������� ������� �����
            if (!isTableOccupied && customerQueue.Count == 1)
            {
                MoveCustomerToTable();
            }
        }
        else
        {
            Debug.LogWarning("������� ���������! ����� ������.");
            customer.MoveTo(exitDoor.position, () => Destroy(customer.gameObject));
        }
    }

    public bool IsQueueFull()
    {
        return customerQueue.Count >= queuePositions.Count;
    }

    
    private void MoveCustomerToTable()
    {
        if (customerQueue.Count == 0) return;

        currentCustomer = customerQueue.Dequeue();
        isTableOccupied = true;

        // ��������� ��������� ������
        AssignRandomRequest(currentCustomer);

        currentCustomer.MoveTo(table.position, () =>
        {
            Debug.Log("����� ���� �������������� � ������.");
            satisfactionBar.StartDecreasing();
            
            // ���������� ������ ����������
            currentCustomer.ShowRequest();
        });
        currentCustomer.SetReadyForInteraction(true);
        
        UpdateQueuePositions();
    }

    // ����� ��� ���������� ���������� �������
    private void AssignRandomRequest(Customer customer)
    {
        // ���������� ��������� ���������� �� 1 �� 5
        int randomQuantity = UnityEngine.Random.Range(1, 6);
        customer.RequestedQuantity = randomQuantity;
    
        if (ProductManager.Instance != null)
        {
            // �������� ��������� ���� ������� �� ProductManager
            List<Item.ItemType> availableTypes = ProductManager.Instance.GetAllProductTypes();
        
            if (availableTypes.Count > 0)
            {
                // �������� ��������� �����
                int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);
                customer.RequestedItem = availableTypes[randomIndex];
                return;
            }
        }
    
        // �������� �������, ���� ProductManager ����������
        int itemTypes = System.Enum.GetValues(typeof(Item.ItemType)).Length;
        Item.ItemType randomItem = (Item.ItemType)UnityEngine.Random.Range(1, itemTypes); // ������� � 1, ����� ���������� None
        customer.RequestedItem = randomItem;
    }

    public void HandleHeroInteraction()
    {
        if (currentCustomer != null && currentCustomer.IsReadyForInteraction())
        {
            Debug.Log("����� ��������������� � ������.");
            
            // �������� ������ ����������
            currentCustomer.HideRequest();
            
            // ������������ ���
            satisfactionBar.StopDecreasing();
            satisfactionBar.NewBar();
            currentCustomer.SetReadyForInteraction(false);
            currentCustomer.MoveTo(exitDoor.position, () =>
            {
                Destroy(currentCustomer.gameObject);
                isTableOccupied = false;
                currentCustomer = null;
                MoveCustomerToTable();
            });

            OnQueueSpaceAvailable?.Invoke();
        }
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (Customer customer in customerQueue)
        {
            customer.MoveTo(queuePositions[index].position);
            index++;
        }
    }
    
    public Customer GetCurrentCustomer()
    {
        return currentCustomer;
    }
}