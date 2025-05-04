using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform productContainer;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private Button closeButton;

    private Hero playerHero;
    private bool isShopOpen = false;
    private List<ShopItem> cachedShopItems = new List<ShopItem>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (playerHero == null)
            playerHero = FindObjectOfType<Hero>();

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);

        UpdatePlayerMoneyDisplay();
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);

        if (isShopOpen)
        {
            PopulateShop();
            UpdatePlayerMoneyDisplay();
            RefreshPrices();
        }
    }

    private void PopulateShop()
    {
        foreach (Transform child in productContainer)
            Destroy(child.gameObject);

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
                    cachedShopItems.Add(shopItem);
                }
            }
        }
    }

    public void RefreshPrices()
    {
        if (!isShopOpen || productContainer == null)
            return;

        foreach (ShopItem shopItem in cachedShopItems)
        {
            shopItem.RefreshPrice();
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

        int productCost = ProductManager.Instance.GetProductBuyingPrice(productType);
        int totalCost = productCost * quantity;

        if (playerHero.balance >= totalCost)
        {
            playerHero.AddMoney(-totalCost);
            UpdatePlayerMoneyDisplay();

            Shelf[] shelves = FindObjectsOfType<Shelf>();
            bool addedToShelf = false;

            foreach (Shelf shelf in shelves)
            {
                if (shelf.item == productType)
                {
                    shelf.AddItems(quantity);
                    addedToShelf = true;
                }
            }

            if (!addedToShelf)
            {
                playerHero.AddMoney(totalCost);
                Debug.Log("Не удалось добавить товар на полку: " + productType);
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

    public void CloseShop()
    {
        if (isShopOpen)
        {
            ToggleShop();
        }
    }
}
