using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    #region События и публичные свойства
    
    // Событие для оповещения о свободном месте в очереди
    public event Action OnQueueSpaceAvailable;
    
    // Проверка заполненности очереди
    public bool IsQueueFull => customerQueue.Count >= queuePositions.Count;
    
    #endregion
    
    #region Сериализуемые поля
    
    [Header("Настройки бесконечного режима")]
    [SerializeField] private bool infiniteModeEnabled = true;
    [SerializeField] private float delayBetweenWaves = 10f;
    [SerializeField] private float difficultyMultiplier = 0.15f;
    
    [Header("Параметры клиентов")]
    [SerializeField] private float baseCustomerSpeed = 2f;
    [SerializeField] private int baseMaxRequestQuantity = 3;
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private GameObject satisfactionBarPrefab;
    
    [Header("Позиции и точки")]
    [SerializeField] private List<Transform> queuePositions = new List<Transform>();
    [SerializeField] private Transform table;
    [SerializeField] private Transform exitDoor;
    [SerializeField] private Transform entranceDoor;
    
    #endregion
    
    #region Приватные поля
    
    private Queue<Customer> customerQueue = new Queue<Customer>();
    private Customer currentCustomer;
    private bool isTableOccupied = false;
    private int currentWave = 1;
    private Transform customersContainer;
    
    #endregion
    
    #region Методы жизненного цикла Unity
    
    private void Awake()
    {
        // Находим или создаем контейнер для клиентов
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
    
    #region Публичные методы
    
    /// <summary>
    /// Добавляет клиента в очередь
    /// </summary>
    public void AddCustomerToQueue(Customer customer)
    {
        if (IsQueueFull)
        {
            Debug.LogWarning("Очередь заполнена! Клиент уходит.");
            SendCustomerToExit(customer);
            return;
        }
        
        customerQueue.Enqueue(customer);
        UpdateQueuePositions();
        
        // Если стол свободен и это первый клиент, отправляем его к столу
        if (!isTableOccupied && customerQueue.Count == 1)
        {
            MoveCustomerToTable();
        }
    }
    
    /// <summary>
    /// Обработка взаимодействия героя с клиентом
    /// </summary>
    public void HandleHeroInteraction()
    {
        if (currentCustomer == null || !currentCustomer.IsReadyForInteraction())
            return;
        
        Debug.Log("Герой взаимодействует с клиентом.");
        
        // Скрываем запрос покупателя
        currentCustomer.HideRequest();
        
        // Останавливаем уменьшение удовлетворенности
        currentCustomer.StopSatisfactionDecrease();
        
        // Сохраняем ссылку на уходящего клиента
        Customer customerToExit = currentCustomer;
        
        // Освобождаем стол
        currentCustomer.SetReadyForInteraction(false);
        isTableOccupied = false;
        currentCustomer = null;
        
        // Перемещаем следующего клиента к столу
        MoveCustomerToTable();
        
        // Отправляем предыдущего клиента к выходу
        SendCustomerToExit(customerToExit);
        
        // Уведомляем о свободном месте в очереди
        OnQueueSpaceAvailable?.Invoke();
        
        // Проверяем статус очереди
        CheckQueueStatus();
    }
    
    /// <summary>
    /// Обработка ухода клиента по окончании терпения
    /// </summary>
    public void HandleCustomerLeave(Customer leavingCustomer)
    {
        if (leavingCustomer != currentCustomer)
            return;
        
        // Скрываем запрос покупателя
        leavingCustomer.HideRequest();
        
        // Освобождаем стол
        isTableOccupied = false;
        currentCustomer = null;
        
        // Отправляем клиента к выходу
        SendCustomerToExit(leavingCustomer);
        
        // Перемещаем следующего клиента к столу
        MoveCustomerToTable();
        
        // Уведомляем о свободном месте в очереди
        OnQueueSpaceAvailable?.Invoke();
    }
    
    /// <summary>
    /// Получение текущего клиента у стола
    /// </summary>
    public Customer GetCurrentCustomer()
    {
        return currentCustomer;
    }
    
    #endregion
    
    #region Приватные методы
    
    /// <summary>
    /// Проверка состояния очереди для запуска новой волны
    /// </summary>
    private void CheckQueueStatus()
    {
        if (infiniteModeEnabled && customerQueue.Count == 0 && currentCustomer == null)
        {
            if (!IsInvoking("GenerateNextWaveAfterDelay"))
            {
                Debug.Log($"Очередь закончилась. Следующая волна через {delayBetweenWaves} секунд...");
                Invoke("GenerateNextWaveAfterDelay", delayBetweenWaves);
            }
        }
    }
    
    /// <summary>
    /// Запуск генерации следующей волны после задержки
    /// </summary>
    private void GenerateNextWaveAfterDelay()
    {
        currentWave++;
        Debug.Log($"Начинаем генерацию волны {currentWave}");
        GenerateNextWave();
    }
    
    /// <summary>
    /// Генерация новой волны клиентов
    /// </summary>
    private void GenerateNextWave()
    {
        try
        {
            float difficulty = 1f + (currentWave - 1) * difficultyMultiplier;
            
            // Определяем количество клиентов
            int customersCount = Mathf.FloorToInt(5 * difficulty);
            customersCount = Mathf.Clamp(customersCount, 1, queuePositions.Count);
            
            // Определяем максимальное количество товаров в запросе
            int maxRequestQuantity = Mathf.FloorToInt(baseMaxRequestQuantity * difficulty);
            maxRequestQuantity = Mathf.Clamp(maxRequestQuantity, 1, 10);
            
            // Определяем скорость клиентов
            float customerSpeed = baseCustomerSpeed * (1f + (currentWave - 1) * 0.1f);
            
            Debug.Log($"Волна {currentWave}: клиентов {customersCount}, максимальное количество товаров {maxRequestQuantity}, скорость {customerSpeed:F1}");
            
            // Создаем клиентов с небольшой задержкой
            StartCoroutine(SpawnCustomersWithDelay(customersCount, customerSpeed, maxRequestQuantity));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при генерации волны: {e.Message}");
        }
    }
    
    /// <summary>
    /// Создание клиентов с задержкой
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
    /// Создание одного клиента
    /// </summary>
    private void SpawnCustomer(float speed, int maxRequestQuantity)
    {
        if (customerPrefab == null || entranceDoor == null)
        {
            Debug.LogError("customerPrefab или entranceDoor не найдены!");
            return;
        }
        
        try
        {
            // Создаем клиента у входа
            GameObject customerObj = Instantiate(customerPrefab, entranceDoor.position, Quaternion.identity, customersContainer);
            Customer customer = customerObj.GetComponent<Customer>();
            
            if (customer != null)
            {
                // Настраиваем параметры клиента
                customer.moveSpeed = speed;
                
                // Создаем и назначаем SatisfactionBar
                if (satisfactionBarPrefab != null)
                {
                    GameObject barObj = Instantiate(satisfactionBarPrefab, customersContainer);
                    SatisfactionBar bar = barObj.GetComponent<SatisfactionBar>();
                    
                    if (bar != null)
                    {
                        customer.SetSatisfactionBar(bar);
                    }
                }
                
                // Назначаем случайный запрос
                AssignRandomRequest(customer, maxRequestQuantity);
                
                // Добавляем клиента в очередь
                AddCustomerToQueue(customer);
                
                Debug.Log($"Клиент создан и добавлен в очередь");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при создании клиента: {e.Message}");
        }
    }
    
    /// <summary>
    /// Назначение случайного запроса клиенту
    /// </summary>
    private void AssignRandomRequest(Customer customer, int maxRequestQuantity)
    {
        // Генерируем случайное количество товара
        int randomQuantity = UnityEngine.Random.Range(1, maxRequestQuantity + 1);
        customer.RequestedQuantity = randomQuantity;
        
        // Используем ProductManager для получения доступных товаров
        if (ProductManager.Instance != null)
        {
            List<Item.ItemType> availableTypes = ProductManager.Instance.GetAllProductTypes();
            availableTypes.RemoveAll(type => type == Item.ItemType.None);
            
            if (availableTypes.Count > 0)
            {
                // Выбираем случайный товар
                int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);
                customer.RequestedItem = availableTypes[randomIndex];
                Debug.Log($"Клиенту назначен товар: {customer.RequestedItem}, количество: {customer.RequestedQuantity}");
                return;
            }
        }
        
        // Запасной вариант, если ProductManager недоступен
        Item.ItemType[] types = { Item.ItemType.Potato, Item.ItemType.Cucumber, Item.ItemType.Tomato, Item.ItemType.Corn, Item.ItemType.Eggplant };
        int index = UnityEngine.Random.Range(0, types.Length);
        customer.RequestedItem = types[index];
    }
    
    /// <summary>
    /// Перемещение клиента к столу
    /// </summary>
    private void MoveCustomerToTable()
    {
        if (customerQueue.Count == 0) return;
        
        currentCustomer = customerQueue.Dequeue();
        isTableOccupied = true;
        
        // Перемещаем клиента к столу
        currentCustomer.MoveTo(table.position, () =>
        {
            Debug.Log("Клиент ждет взаимодействия с героем.");
            
            // Показываем запрос покупателя
            currentCustomer.ShowRequest();
            
            // Запускаем уменьшение удовлетворенности
            currentCustomer.StartSatisfactionDecrease();
        });
        
        currentCustomer.SetReadyForInteraction(true);
        UpdateQueuePositions();
    }
    
    /// <summary>
    /// Обновление позиций клиентов в очереди
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
    /// Отправка клиента к выходу
    /// </summary>
    private void SendCustomerToExit(Customer customer)
    {
        if (customer == null) return;
        
        customer.MoveTo(exitDoor.position, () =>
        {
            // Уничтожаем SatisfactionBar вместе с клиентом
            if (customer.GetSatisfactionBar() != null)
            {
                Destroy(customer.GetSatisfactionBar().gameObject);
            }
            
            Destroy(customer.gameObject);
        });
    }
    
    #endregion
}
