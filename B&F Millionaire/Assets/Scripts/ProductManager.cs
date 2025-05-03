using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    // Реализация синглтона
    public static ProductManager Instance { get; private set; }

    // Структура данных о продукте
    [System.Serializable]
    public struct ProductData
    {
        public string Name;
        public Sprite Icon;
        public int SellingPrice; // Стоимость продажи покупателям
        public int BuyingPrice;  // Стоимость покупки у поставщика
        public Item.ItemType ItemType;
    }

    // Список продуктов для настройки в инспекторе
    [SerializeField] private List<ProductData> products = new List<ProductData>();
    
    // Словарь для быстрого доступа к данным
    private Dictionary<Item.ItemType, ProductData> productDictionary = new Dictionary<Item.ItemType, ProductData>();

    private void Awake()
    {
        // Проверка для синглтона
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeProductDictionary();
        }
    }

    // Инициализация словаря продуктов
    private void InitializeProductDictionary()
    {
        productDictionary.Clear();
        foreach (var product in products)
        {
            productDictionary[product.ItemType] = product;
        }
    }

    // Получение данных о продукте
    public ProductData GetProductData(Item.ItemType itemType)
    {
        if (productDictionary.TryGetValue(itemType, out ProductData data))
        {
            return data;
        }
        
        Debug.LogWarning($"Продукт с типом {itemType} не найден.");
        return new ProductData();
    }

    // Получение списка всех типов товаров
    public List<Item.ItemType> GetAllProductTypes()
    {
        List<Item.ItemType> types = new List<Item.ItemType>();
        foreach (var product in products)
        {
            types.Add(product.ItemType);
        }
        return types;
    }

    // Получение спрайта для товара
    public Sprite GetProductSprite(Item.ItemType itemType)
    {
        if (productDictionary.TryGetValue(itemType, out ProductData data))
        {
            return data.Icon;
        }
        return null;
    }

    // Получение названия товара
    public string GetProductName(Item.ItemType itemType)
    {
        if (productDictionary.TryGetValue(itemType, out ProductData data))
        {
            return data.Name;
        }
        return "Неизвестный товар";
    }

    // Получение стоимости продажи товара
    public int GetProductSellingPrice(Item.ItemType itemType)
    {
        if (productDictionary.TryGetValue(itemType, out ProductData data))
        {
            return data.SellingPrice;
        }
        return 0;
    }

    // Получение стоимости покупки товара
    public int GetProductBuyingPrice(Item.ItemType itemType)
    {
        if (productDictionary.TryGetValue(itemType, out ProductData data))
        {
            return data.BuyingPrice;
        }
        return 0;
    }

    // Для обратной совместимости (используется в существующем коде)
    public int GetProductCost(Item.ItemType itemType)
    {
        return GetProductSellingPrice(itemType);
    }
}
