using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab; // Префаб гостя
    [SerializeField] private Transform spawnPoint; // Точка появления гостей
    [SerializeField] private float spawnInterval = 5f; // Интервал появления
    [SerializeField] private int maxCustomersPerDay; // Максимальное количество покупателей за день
    [SerializeField] private GameObject Customers;

    private QueueManager queueManager;
    private bool canSpawn = true; // Флаг для контроля генерации
    private int customersSpawned = 0; // Счётчик количества покупателей

    private void Start()
    {
        queueManager = FindObjectOfType<QueueManager>();
        queueManager.OnQueueSpaceAvailable += HandleQueueSpaceAvailable; // Подписка на событие

        StartCoroutine(SpawnCustomers());
    }

    private IEnumerator SpawnCustomers()
    {
        while (customersSpawned < maxCustomersPerDay) // Проверяем, не превышен ли лимит
        {
            yield return new WaitUntil(() => canSpawn); // Ждем, пока спавн разрешен

            if (queueManager.IsQueueFull())
            {
                canSpawn = false; // Останавливаем генерацию, если очередь заполнена
                yield return null;
            }
            else
            {
                // Создаем нового гостя
                GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                newCustomer.transform.SetParent(Customers.transform);
                Customer customerScript = newCustomer.GetComponent<Customer>();
                queueManager.AddCustomerToQueue(customerScript);

                customersSpawned++; // Увеличиваем счетчик созданных покупателей

                yield return new WaitForSeconds(spawnInterval); // Интервал перед следующей генерацией
            }
        }

        Debug.Log("Лимит покупателей за день достигнут.");
    }

    private void HandleQueueSpaceAvailable()
    {
        canSpawn = true; // Разрешаем генерацию, когда появляется место
    }

    private void OnDestroy()
    {
        queueManager.OnQueueSpaceAvailable -= HandleQueueSpaceAvailable; // Отписка от события
    }
}
