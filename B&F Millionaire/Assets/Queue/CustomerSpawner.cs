using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab; // ������ �����
    [SerializeField] private Transform spawnPoint; // ����� ��������� ������
    [SerializeField] private float spawnInterval = 5f; // �������� ���������
    [SerializeField] private int maxCustomersPerDay; // ������������ ���������� ����������� �� ����
    [SerializeField] private GameObject Customers;

    private QueueManager queueManager;
    private bool canSpawn = true; // ���� ��� �������� ���������
    private int customersSpawned = 0; // ������� ���������� �����������

    private void Start()
    {
        queueManager = FindObjectOfType<QueueManager>();
        queueManager.OnQueueSpaceAvailable += HandleQueueSpaceAvailable; // �������� �� �������

        StartCoroutine(SpawnCustomers());
    }

    private IEnumerator SpawnCustomers()
    {
        while (customersSpawned < maxCustomersPerDay) // ���������, �� �������� �� �����
        {
            yield return new WaitUntil(() => canSpawn); // ����, ���� ����� ��������

            if (queueManager.IsQueueFull())
            {
                canSpawn = false; // ������������� ���������, ���� ������� ���������
                yield return null;
            }
            else
            {
                // ������� ������ �����
                GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                newCustomer.transform.SetParent(Customers.transform);
                Customer customerScript = newCustomer.GetComponent<Customer>();
                queueManager.AddCustomerToQueue(customerScript);

                customersSpawned++; // ����������� ������� ��������� �����������

                yield return new WaitForSeconds(spawnInterval); // �������� ����� ��������� ����������
            }
        }

        Debug.Log("����� ����������� �� ���� ���������.");
    }

    private void HandleQueueSpaceAvailable()
    {
        canSpawn = true; // ��������� ���������, ����� ���������� �����
    }

    private void OnDestroy()
    {
        queueManager.OnQueueSpaceAvailable -= HandleQueueSpaceAvailable; // ������� �� �������
    }
}
