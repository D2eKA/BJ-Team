using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform productContainer;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private Button closeButton;
    
    private Hero playerHero;
    private bool isShopOpen = false;

    private void Start()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        // Находим героя
        if (playerHero == null)
            playerHero = FindObjectOfType<Hero>();
            
        // Настраиваем кнопку закрытия
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
            
        UpdatePlayerMoneyDisplay();
    }

    private void Update()
    {
        // Открытие магазина по клавише B
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);
        
        if (isShopOpen)
        {
            PopulateShop();
            UpdatePlayerMoneyDisplay();
        }
    }
    
    public void CloseShop()
    {
        if (isShopOpen)
        {
            ToggleShop();
        }
    }

    private void PopulateShop()
    {
        // Очищаем контейнер
        foreach (Transform child in productContainer)
        {
            Destroy(child.gameObject);
        }

        if (ProductManager.Instance != null)
        {
            List<Item.ItemType> productTypes = ProductManager.Instance.GetAllProductTypes();
            
            foreach (Item.ItemType type in productTypes)
            {
                if (type == Item.ItemType.None)
                    continue;
                    
                GameObject productItem = Instantiate(productPrefab, productContainer);
                ShopItem shopItem = productItem.GetComponent<ShopItem>();
                
                if (shopItem != null)
                {
                    shopItem.Initialize(type);
                }
            }
        }
    }

    public void UpdatePlayerMoneyDisplay()
    {
        if (playerMoneyText != null && playerHero != null)
        {
            playerMoneyText.text = "Деньги: " + playerHero.balance.ToString();
        }
    }

    public void BuyProduct(Item.ItemType productType, int quantity)
    {
        if (ProductManager.Instance == null || playerHero == null)
            return;
        
        int productCost = ProductManager.Instance.GetProductBuyingPrice(productType); // Используем цену покупки
        int totalCost = productCost * quantity;
    
        if (playerHero.balance >= totalCost)
        {
            // Вычитаем деньги у игрока
            playerHero.AddMoney(-totalCost);
        
            // Обновляем отображение денег в магазине
            UpdatePlayerMoneyDisplay();
        
            // Находим все полки с данным типом товара
            Shelf[] shelves = FindObjectsOfType<Shelf>();
            bool addedToShelf = false;
        
            foreach (Shelf shelf in shelves)
            {
                if (shelf.item == productType)
                {
                    // Добавляем товар на полку
                    shelf.AddItems(quantity);
                    addedToShelf = true;
                    // Не прерываем цикл, чтобы пополнить все полки этого типа
                }
            }
        
            if (!addedToShelf)
            {
                // Если нет подходящей полки, возвращаем деньги
                playerHero.AddMoney(totalCost);
                Debug.Log("Не найдено подходящей полки для товара " + productType);
            }
            else
            {
                Debug.Log($"Куплено: {productType} x{quantity} за {totalCost} монет");
            }
        }
        else
        {
            Debug.Log("Недостаточно денег для покупки");
        }
    }
}
