using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // Позиции в очереди
    [SerializeField] private Transform table; // Стол
    [SerializeField] private Transform exitDoor; // Выходная дверь
    private SatisfactionBar satisfactionBar; //Шкала счастья

    private Queue<Customer> customerQueue = new Queue<Customer>(); // Очередь гостей
    private Customer currentCustomer; // Гость у стола
    public bool isTableOccupied = false; // Занят ли стол

    public event Action OnQueueSpaceAvailable; // Событие для уведомления о доступности места\

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

            // Если стол свободен, перемещаем первого гостя
            if (!isTableOccupied && customerQueue.Count == 1)
            {
                MoveCustomerToTable();
            }
        }
        else
        {
            Debug.LogWarning("Очередь заполнена! Гость уходит.");
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

        // Назначаем случайный запрос
        AssignRandomRequest(currentCustomer);

        currentCustomer.MoveTo(table.position, () =>
        {
            Debug.Log("Гость ждет взаимодействия с героем.");
            satisfactionBar.StartDecreasing();
            
            // Показываем запрос покупателя
            currentCustomer.ShowRequest();
        });
        currentCustomer.SetReadyForInteraction(true);
        
        UpdateQueuePositions();
    }

    // Метод для назначения случайного запроса
    private void AssignRandomRequest(Customer customer)
    {
        // Генерируем случайное количество от 1 до 5
        int randomQuantity = UnityEngine.Random.Range(1, 6);
        customer.RequestedQuantity = randomQuantity;
    
        if (ProductManager.Instance != null)
        {
            // Получаем доступные типы товаров из ProductManager
            List<Item.ItemType> availableTypes = ProductManager.Instance.GetAllProductTypes();
        
            if (availableTypes.Count > 0)
            {
                // Выбираем случайный товар
                int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);
                customer.RequestedItem = availableTypes[randomIndex];
                return;
            }
        }
    
        // Запасной вариант, если ProductManager недоступен
        int itemTypes = System.Enum.GetValues(typeof(Item.ItemType)).Length;
        Item.ItemType randomItem = (Item.ItemType)UnityEngine.Random.Range(1, itemTypes); // Начиная с 1, чтобы пропустить None
        customer.RequestedItem = randomItem;
    }

    public void HandleHeroInteraction()
    {
        if (currentCustomer != null && currentCustomer.IsReadyForInteraction())
        {
            Debug.Log("Герой взаимодействует с гостем.");
            
            // Скрываем запрос покупателя
            currentCustomer.HideRequest();
            
            // Существующий код
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