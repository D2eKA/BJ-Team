using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // ������� � �������
    [SerializeField] private Transform table; // ����
    [SerializeField] private Transform exitDoor; // �������� �����
    public SatisfactionBar satisfactionBar; //����� �������

    private Queue<Customer> customerQueue = new Queue<Customer>(); // ������� ������
    private Customer currentCustomer; // ����� � �����
    public bool isTableOccupied = false; // ����� �� ����

    public event Action OnQueueSpaceAvailable; // ������� ��� ����������� � ����������� �����

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

        currentCustomer.MoveTo(table.position, () =>
        {
            Debug.Log("����� ���� �������������� � ������.");
            satisfactionBar.StartDecreasing();
        });
        currentCustomer.SetReadyForInteraction(true);
        
        UpdateQueuePositions();
    }

    public void HandleHeroInteraction()
    {
        if (currentCustomer != null && currentCustomer.IsReadyForInteraction())
        {
            Debug.Log("����� ��������������� � ������.");
            ////
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
}