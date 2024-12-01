using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // Позиции в очереди
    [SerializeField] private Transform table; // Стол
    [SerializeField] private Transform exitDoor; // Выходная дверь

    private Queue<Customer> customerQueue = new Queue<Customer>(); // Очередь гостей
    private Customer currentCustomer; // Гость у стола
    private bool isTableOccupied = false; // Занят ли стол

    public event Action OnQueueSpaceAvailable; // Событие для уведомления о доступности места

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

        currentCustomer.MoveTo(table.position, () =>
        {
            Debug.Log("Гость ждет взаимодействия с героем.");
            currentCustomer.SetReadyForInteraction(true);
        });

        UpdateQueuePositions();
    }

    public void HandleHeroInteraction()
    {
        if (currentCustomer != null && currentCustomer.IsReadyForInteraction())
        {
            Debug.Log("Герой взаимодействует с гостем.");
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
}