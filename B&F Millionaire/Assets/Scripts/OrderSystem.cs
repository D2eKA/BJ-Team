using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Чтобы можно было редактировать в инспекторе Unity
public class Product
{
    public string productName; // Название продукта
    public float costPrice;    // Себестоимость
    public float sellPrice;    // Цена продажи

    // Конструктор для удобного создания продукта
    public Product(string name, float cost, float sell)
    {
        productName = name;
        costPrice = cost;
        sellPrice = sell;
    }
}



public class BuyerOrderHandler : MonoBehaviour
{
    public List<Product> currentOrder; // Текущий заказ покупателя

    // Метод для установки заказа
    public void SetOrder(List<Product> order)
    {
        currentOrder = order;
        Debug.Log("Order assigned to buyer: " + order.Count + " items.");
    }

    // Метод для проверки выполнения заказа
    public bool CheckOrder(List<Product> playerOrder)
    {
        // Логика сравнения заказов
        // Например, проверка, что playerOrder совпадает с currentOrder
        return true; // Заглушка
    }
}

public class OrderSystem : MonoBehaviour
{
    // Список доступных продуктов
    public List<Product> availableProducts = new List<Product>
    {
        new Product("Apple", 1.0f, 2.0f),
        new Product("Bread", 2.0f, 3.5f),
        new Product("Milk", 1.5f, 3.0f)
    };

    // Список всех покупателей на сцене
    private List<GameObject> buyers = new List<GameObject>();

    void Start()
    {
        // Находим всех покупателей на сцене (например, по тегу)
        buyers.AddRange(GameObject.FindGameObjectsWithTag("Buyer"));

        // Назначаем заказы каждому покупателю
        AssignOrdersToBuyers();
    }

    // Метод для генерации заказа
    public List<Product> GenerateOrder()
    {
        List<Product> order = new List<Product>();
        int orderSize = Random.Range(1, 4); // Размер заказа (1-3 продукта)

        for (int i = 0; i < orderSize; i++)
        {
            Product randomProduct = availableProducts[Random.Range(0, availableProducts.Count)];
            order.Add(randomProduct);
        }

        Debug.Log("New order generated with " + orderSize + " items.");
        return order;
    }

    // Метод для назначения заказов покупателям
    void AssignOrdersToBuyers()
    {
        foreach (GameObject buyer in buyers)
        {
            List<Product> order = GenerateOrder();

            // Передаем заказ покупателю (например, через компонент или публичный метод)
            // Если у покупателя есть компонент, который может принимать заказ:
            BuyerOrderHandler orderHandler = buyer.GetComponent<BuyerOrderHandler>();
            if (orderHandler != null)
            {
                orderHandler.SetOrder(order);
            }
            else
            {
                Debug.LogWarning("Buyer does not have a BuyerOrderHandler component!");
            }
        }
    }
}