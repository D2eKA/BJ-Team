using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("�������� ���������")]
    public float moveSpeed = 2f;
    
    [Header("���������� ��������")]
    [SerializeField] private SpriteRenderer requestIconRenderer;
    [SerializeField] private GameObject requestBubble;
    [SerializeField] private TextMeshProUGUI requestQuantityText;
    [SerializeField]  private SatisfactionBar satisfactionBar;
    
    // ��������� ���� ��� ��������� �������
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Action onReachedTarget;
    private bool readyForInteraction = false;
    
    private Item.ItemType requestedItem = Item.ItemType.None;
    private int requestedQuantity = 1;

    // �������� ��� ������� � �������������� ������
    public Item.ItemType RequestedItem
    {
        get { return requestedItem; }
        set 
        { 
            requestedItem = value;
            UpdateRequestIcon();
        }
    }
    
    // �������� ��� ������� � ����������
    public int RequestedQuantity
    {
        get { return requestedQuantity; }
        set 
        { 
            requestedQuantity = value;
            UpdateRequestQuantityText();
        }
    }
    
    // ����� �������������
    private void Start()
    {
        // �������� ����������� �����������
        ValidateComponents();
        
        // �� ��������� �������� ������ �������
        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }
    
    // �������� ������� ����������� �����������
    private void ValidateComponents()
    {
        if (requestIconRenderer == null)
        {
            Debug.LogError($"requestIconRenderer �� �������� � {gameObject.name}!");
        }
        
        if (requestBubble == null)
        {
            Debug.LogError($"requestBubble �� �������� � {gameObject.name}!");
        }
        
        if (requestQuantityText == null)
        {
            Debug.LogError($"requestQuantityText �� �������� � {gameObject.name}!");
        }
    }
    
    // ��������� �������� SatisfactionBar
    public SatisfactionBar GetSatisfactionBar()
    {
        return satisfactionBar;
    }
    
    // ����� ��� ��������� ������ �� SatisfactionBar
    public void SetSatisfactionBar(SatisfactionBar bar)
    {
        // ��������� ���������� SatisfactionBar, ���� �� ���
        if (satisfactionBar != null)
        {
            satisfactionBar.StopDecreasing();
            satisfactionBar.customer = null;
        }
        
        satisfactionBar = bar;
        if (satisfactionBar != null)
        {
            satisfactionBar.customer = this;
            satisfactionBar.NewBar(); // ���������� �������� ����
        }
    }
    
    // ������ ��� ���������� ������� �����������������
    public void StartSatisfactionDecrease()
    {
        if (satisfactionBar != null)
        {
            satisfactionBar.StartDecreasing();
        }
        else
        {
            Debug.LogWarning($"������� ��������� SatisfactionBar, �� �� �� �������� ��� {gameObject.name}");
        }
    }
    
    public void StopSatisfactionDecrease()
    {
        if (satisfactionBar != null)
        {
            satisfactionBar.StopDecreasing();
            satisfactionBar.NewBar();
        }
    }
    
    // ����� ��� ����� ������� (���������� �� SatisfactionBar)
    public void LeaveFromQueue()
    {
        QueueManager queueManager = FindObjectOfType<QueueManager>();
        if (queueManager != null)
        {
            queueManager.HandleCustomerLeave(this);
        }
        else
        {
            Debug.LogError("QueueManager �� ������ ��� ������� ����� �������!");
        }
    }

    // ���������� ������ �������
    private void UpdateRequestIcon()
    {
        if (requestIconRenderer == null)
        {
            Debug.LogError("requestIconRenderer �� ��������!");
            return;
        }
        
        if (ProductManager.Instance == null)
        {
            Debug.LogError("ProductManager.Instance �� ��������!");
            return;
        }
        
        if (requestedItem == Item.ItemType.None)
        {
            Debug.LogWarning("��� �������������� ������ - None!");
            return;
        }
        
        Sprite productSprite = ProductManager.Instance.GetProductSprite(requestedItem);
        if (productSprite != null)
        {
            requestIconRenderer.sprite = productSprite;
        }
        else
        {
            Debug.LogError($"�� ������� �������� ������ ��� ������ ���� {requestedItem}");
        }
    }

    // ����� ��� ���������� ������ ����������
    private void UpdateRequestQuantityText()
    {
        if (requestQuantityText != null)
        {
            requestQuantityText.text = "x" + requestedQuantity.ToString();
        }
    }

    // �������� ������
    public void ShowRequest()
    {
        if (requestBubble == null)
        {
            Debug.LogError("requestBubble �����������!");
            return;
        }
        
        if (requestedItem == Item.ItemType.None)
        {
            Debug.LogWarning("������� �������� ������ ��� ������������ ������!");
            
            // ��������� �������� ����� ���� ��������
            TryAssignRandomItem();
        }
        
        UpdateRequestIcon();
        UpdateRequestQuantityText();
        requestBubble.SetActive(true);
        
        Debug.Log($"������� ������ ������: {requestedItem}, ����������: {requestedQuantity}");
    }
    
    // ������� ��������� ��������� ����� �������
    private void TryAssignRandomItem()
    {
        if (ProductManager.Instance != null)
        {
            List<Item.ItemType> availableTypes = ProductManager.Instance.GetAllProductTypes();
            availableTypes.RemoveAll(type => type == Item.ItemType.None);
            
            if (availableTypes.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);
                RequestedItem = availableTypes[randomIndex];
                Debug.Log($"������� �������� �������� �����: {RequestedItem}");
            }
            else
            {
                Debug.LogError("��� ��������� ������� ��� ����������!");
                // ��������� �������� �� ��������� ��� �������� �������
                RequestedItem = Item.ItemType.Potato;
            }
        }
        else
        {
            Debug.LogError("ProductManager ���������� ��� ���������� ������!");
            // ��������� �������� �� ��������� ��� �������� �������
            RequestedItem = Item.ItemType.Potato;
        }
    }

    // ������ ������
    public void HideRequest()
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }
    }

    // ������ ������������
    public void MoveTo(Vector3 position, Action onComplete = null)
    {
        targetPosition = position;
        onReachedTarget = onComplete;
        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            onReachedTarget?.Invoke();
        }
    }

    // ������ ��� ��������������
    public bool IsReadyForInteraction()
    {
        return readyForInteraction;
    }

    public void SetReadyForInteraction(bool ready)
    {
        readyForInteraction = ready;
    }
}
