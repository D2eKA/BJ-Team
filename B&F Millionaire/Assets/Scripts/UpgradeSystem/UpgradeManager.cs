using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    
    [SerializeField] private Hero hero;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ProductManager productManager;
    
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private Button closeButton;
    
    [SerializeField] private List<Upgrade> availableUpgrades = new List<Upgrade>();
    
    // Настройка для сохранения улучшений между сессиями
    [SerializeField] private bool persistentUpgrades = true;
    [Tooltip("Если включено, улучшения будут сохраняться между запусками игры")]
    
    private float baseMovementSpeed;
    private int baseInventoryCapacity;
    private Dictionary<UpgradeType, float> appliedUpgrades = new Dictionary<UpgradeType, float>();
    
    public enum UpgradeType
    {
        MovementSpeed,
        InventoryCapacity,
        ProductDiscount
    }
    
    [System.Serializable]
    public class Upgrade
    {
        public UpgradeType Type;
        public string Name;
        public string Description;
        public int[] Costs;
        public float[] Values;
        public int CurrentLevel;
        public int MaxLevel;
        public Sprite Icon;
        
        public int GetCurrentCost()
        {
            return CurrentLevel < Costs.Length ? Costs[CurrentLevel] : 0;
        }
        
        public float GetCurrentValue()
        {
            return CurrentLevel > 0 && CurrentLevel <= Values.Length ? Values[CurrentLevel - 1] : 0;
        }
        
        public float GetNextValue()
        {
            return CurrentLevel < Values.Length ? Values[CurrentLevel] : 0;
        }
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        if (hero == null)
            hero = FindObjectOfType<Hero>();
            
        if (inventory == null && hero != null)
            inventory = hero.GetComponent<Inventory>();
            
        if (productManager == null)
            productManager = FindObjectOfType<ProductManager>();
            
        baseMovementSpeed = hero.speed;
        baseInventoryCapacity = inventory.maxCapacity;
        
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUpgradePanel);
            
        InitializeUpgrades();
        LoadUpgrades();
        ApplyAllUpgrades();
    }
    
    private void InitializeUpgrades()
    {
        if (availableUpgrades.Count == 0)
        {
            // Скорость передвижения
            Upgrade speedUpgrade = new Upgrade
            {
                Type = UpgradeType.MovementSpeed,
                Name = "Скорость передвижения",
                Description = "Увеличивает скорость перемещения персонажа",
                Costs = new int[] { 100, 250, 500, 1000, 2000 },
                Values = new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1.0f },
                CurrentLevel = 0,
                MaxLevel = 5
            };
            
            // Вместимость инвентаря
            Upgrade inventoryUpgrade = new Upgrade
            {
                Type = UpgradeType.InventoryCapacity,
                Name = "Вместительный рюкзак",
                Description = "Увеличивает количество товаров, которые можно носить с собой",
                Costs = new int[] { 150, 300, 600, 1200, 2400 },
                Values = new float[] { 5f, 10f, 15f, 20f, 25f },
                CurrentLevel = 0,
                MaxLevel = 5
            };
            
            // Скидки на товары
            Upgrade discountUpgrade = new Upgrade
            {
                Type = UpgradeType.ProductDiscount,
                Name = "Навыки закупщика",
                Description = "Снижает стоимость закупки товаров у поставщиков",
                Costs = new int[] { 200, 400, 800, 1600, 3200 },
                Values = new float[] { 0.05f, 0.1f, 0.15f, 0.2f, 0.25f },
                CurrentLevel = 0,
                MaxLevel = 5
            };
            
            availableUpgrades.Add(speedUpgrade);
            availableUpgrades.Add(inventoryUpgrade);
            availableUpgrades.Add(discountUpgrade);
        }
    }
    
    private void LoadUpgrades()
    {
        // Загружаем улучшения только если включен постоянный режим
        if (!persistentUpgrades)
            return;
            
        foreach (var upgrade in availableUpgrades)
        {
            string key = "Upgrade_" + upgrade.Type.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                upgrade.CurrentLevel = PlayerPrefs.GetInt(key, 0);
            }
        }
    }
    
    private void SaveUpgrades()
    {
        // Сохраняем улучшения только если включен постоянный режим
        if (!persistentUpgrades)
            return;
            
        foreach (var upgrade in availableUpgrades)
        {
            string key = "Upgrade_" + upgrade.Type.ToString();
            PlayerPrefs.SetInt(key, upgrade.CurrentLevel);
        }
        PlayerPrefs.Save();
    }
    
    // Метод для сброса всех улучшений при начале новой игры
    public void ResetUpgrades()
    {
        if (!persistentUpgrades)
        {
            foreach (var upgrade in availableUpgrades)
            {
                upgrade.CurrentLevel = 0;
            }
            ApplyAllUpgrades();
        }
    }
    
    public void ToggleUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(!upgradePanel.activeSelf);
            if (upgradePanel.activeSelf)
                PopulateUpgradePanel();
        }
    }
    
    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }
    
    private void PopulateUpgradePanel()
    {
        foreach (Transform child in upgradeContainer)
            Destroy(child.gameObject);
            
        foreach (var upgrade in availableUpgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeContainer);
            UpgradeButton button = buttonObj.GetComponent<UpgradeButton>();
            if (button != null)
                button.Initialize(upgrade);
        }
    }
    
    public bool PurchaseUpgrade(UpgradeType type)
    {
        Upgrade upgrade = availableUpgrades.Find(u => u.Type == type);
        
        if (upgrade == null || upgrade.CurrentLevel >= upgrade.MaxLevel)
            return false;
            
        int cost = upgrade.GetCurrentCost();
        
        if (hero.balance >= cost)
        {
            hero.AddMoney(-cost);
            upgrade.CurrentLevel++;
            ApplyUpgrade(upgrade);
            SaveUpgrades();
            return true;
        }
        
        return false;
    }
    
    private void ApplyAllUpgrades()
    {
        foreach (var upgrade in availableUpgrades)
        {
            if (upgrade.CurrentLevel > 0)
                ApplyUpgrade(upgrade);
        }
    }
    
    private void ApplyUpgrade(Upgrade upgrade)
    {
        float value = upgrade.GetCurrentValue();
        appliedUpgrades[upgrade.Type] = value;
        
        switch (upgrade.Type)
        {
            case UpgradeType.MovementSpeed:
                hero.speed = baseMovementSpeed * (1 + value);
                break;
                
            case UpgradeType.InventoryCapacity:
                inventory.maxCapacity = baseInventoryCapacity + (int)value;
                inventory.UpdateCapacityDisplay();
                break;
                
            case UpgradeType.ProductDiscount:
                ApplyDiscountToProducts(value);
                break;
        }
    }
    
    private void ApplyDiscountToProducts(float discountPercent)
    {
        if (productManager != null)
        {
            productManager.ApplyDiscount(discountPercent);
            
            // Обновляем цены в магазине, если он открыт
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.RefreshPrices();
            }
        }
    }
    
    public float GetUpgradeValue(UpgradeType type)
    {
        Upgrade upgrade = availableUpgrades.Find(u => u.Type == type);
        return upgrade != null ? upgrade.GetCurrentValue() : 0f;
    }
}
