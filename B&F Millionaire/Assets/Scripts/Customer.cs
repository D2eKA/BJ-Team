using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("Основные настройки")]
    public float moveSpeed = 2f;
    
    [Header("Компоненты запросов")]
    [SerializeField] private SpriteRenderer requestIconRenderer;
    [SerializeField] private GameObject requestBubble;
    [SerializeField] private TextMeshProUGUI requestQuantityText;
    [SerializeField]  private SatisfactionBar satisfactionBar;
    
    // Приватные поля для состояния клиента
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Action onReachedTarget;
    private bool readyForInteraction = false;
    
    private Item.ItemType requestedItem = Item.ItemType.None;
    private int requestedQuantity = 1;

    // Свойство для доступа к запрашиваемому товару
    public Item.ItemType RequestedItem
    {
        get { return requestedItem; }
        set 
        { 
            requestedItem = value;
            UpdateRequestIcon();
        }
    }
    
    // Свойство для доступа к количеству
    public int RequestedQuantity
    {
        get { return requestedQuantity; }
        set 
        { 
            requestedQuantity = value;
            UpdateRequestQuantityText();
        }
    }
    
    // Метод инициализации
    private void Start()
    {
        // Проверка необходимых компонентов
        ValidateComponents();
        
        // По умолчанию скрываем пузырь запроса
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
    
    // Проверка наличия необходимых компонентов
    private void ValidateComponents()
    {
        if (requestIconRenderer == null)
        {
            Debug.LogError($"requestIconRenderer не назначен в {gameObject.name}!");
        }
        
        if (requestBubble == null)
        {
            Debug.LogError($"requestBubble не назначен в {gameObject.name}!");
        }
        
        if (requestQuantityText == null)
        {
            Debug.LogError($"requestQuantityText не назначен в {gameObject.name}!");
        }
    }
    
    // Получение текущего SatisfactionBar
    public SatisfactionBar GetSatisfactionBar()
    {
        return satisfactionBar;
    }
    
    // Метод для установки ссылки на SatisfactionBar
    public void SetSatisfactionBar(SatisfactionBar bar)
    {
        // Отключаем предыдущий SatisfactionBar, если он был
        if (satisfactionBar != null)
        {
            satisfactionBar.StopDecreasing();
            satisfactionBar.customer = null;
        }
        
        satisfactionBar = bar;
        if (satisfactionBar != null)
        {
            satisfactionBar.customer = this;
            satisfactionBar.NewBar(); // Сбрасываем значение бара
        }
    }
    
    // Методы для управления полосой удовлетворенности
    public void StartSatisfactionDecrease()
    {
        if (satisfactionBar != null)
        {
            satisfactionBar.StartDecreasing();
        }
        else
        {
            Debug.LogWarning($"Попытка запустить SatisfactionBar, но он не назначен для {gameObject.name}");
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
    
    // Метод для ухода клиента (вызывается из SatisfactionBar)
    public void LeaveFromQueue()
    {
        QueueManager queueManager = FindObjectOfType<QueueManager>();
        if (queueManager != null)
        {
            queueManager.HandleCustomerLeave(this);
        }
        else
        {
            Debug.LogError("QueueManager не найден при попытке ухода клиента!");
        }
    }

    // Обновление иконки запроса
    private void UpdateRequestIcon()
    {
        if (requestIconRenderer == null)
        {
            Debug.LogError("requestIconRenderer не назначен!");
            return;
        }
        
        if (ProductManager.Instance == null)
        {
            Debug.LogError("ProductManager.Instance не доступен!");
            return;
        }
        
        if (requestedItem == Item.ItemType.None)
        {
            Debug.LogWarning("Тип запрашиваемого товара - None!");
            return;
        }
        
        Sprite productSprite = ProductManager.Instance.GetProductSprite(requestedItem);
        if (productSprite != null)
        {
            requestIconRenderer.sprite = productSprite;
        }
        else
        {
            Debug.LogError($"Не удалось получить спрайт для товара типа {requestedItem}");
        }
    }

    // Метод для обновления текста количества
    private void UpdateRequestQuantityText()
    {
        if (requestQuantityText != null)
        {
            requestQuantityText.text = "x" + requestedQuantity.ToString();
        }
    }

    // Показать запрос
    public void ShowRequest()
    {
        if (requestBubble == null)
        {
            Debug.LogError("requestBubble отсутствует!");
            return;
        }
        
        if (requestedItem == Item.ItemType.None)
        {
            Debug.LogWarning("Попытка показать запрос без назначенного товара!");
            
            // Назначаем запасной товар если возможно
            TryAssignRandomItem();
        }
        
        UpdateRequestIcon();
        UpdateRequestQuantityText();
        requestBubble.SetActive(true);
        
        Debug.Log($"Показан запрос товара: {requestedItem}, количество: {requestedQuantity}");
    }
    
    // Попытка назначить случайный товар клиенту
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
                Debug.Log($"Клиенту назначен запасной товар: {RequestedItem}");
            }
            else
            {
                Debug.LogError("Нет доступных товаров для назначения!");
                // Назначаем картошку по умолчанию как запасной вариант
                RequestedItem = Item.ItemType.Potato;
            }
        }
        else
        {
            Debug.LogError("ProductManager недоступен для назначения товара!");
            // Назначаем картошку по умолчанию как запасной вариант
            RequestedItem = Item.ItemType.Potato;
        }
    }

    // Скрыть запрос
    public void HideRequest()
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }
    }

    // Методы передвижения
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

    // Методы для взаимодействия
    public bool IsReadyForInteraction()
    {
        return readyForInteraction;
    }

    public void SetReadyForInteraction(bool ready)
    {
        readyForInteraction = ready;
    }
}
