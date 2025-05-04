using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Button purchaseButton;
    
    private UpgradeManager.Upgrade upgradeData;
    private UpgradeManager.UpgradeType upgradeType;
    
    public void Initialize(UpgradeManager.Upgrade upgrade)
    {
        upgradeData = upgrade;
        upgradeType = upgrade.Type;
        
        if (upgradeIcon != null && upgrade.Icon != null)
            upgradeIcon.sprite = upgrade.Icon;
            
        if (nameText != null)
            nameText.text = upgrade.Name;
            
        if (descriptionText != null)
            descriptionText.text = upgrade.Description;
            
        UpdateUI();
        
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }
    
    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = $"Уровень: {upgradeData.CurrentLevel}/{upgradeData.MaxLevel}";
            
        bool isMaxLevel = upgradeData.CurrentLevel >= upgradeData.MaxLevel;
        
        if (costText != null)
        {
            if (!isMaxLevel)
                costText.text = $"Стоимость: {upgradeData.GetCurrentCost()}";
            else
                costText.text = "МАКС. УРОВЕНЬ";
        }
        
        if (valueText != null)
        {
            if (!isMaxLevel)
            {
                string valueFormatted = FormatUpgradeValue(upgradeData.Type, upgradeData.GetNextValue());
                valueText.text = $"Следующий: {valueFormatted}";
            }
            else
            {
                string valueFormatted = FormatUpgradeValue(upgradeData.Type, upgradeData.GetCurrentValue());
                valueText.text = $"Текущий: {valueFormatted}";
            }
        }
        
        if (purchaseButton != null)
        {
            purchaseButton.interactable = !isMaxLevel;
            
            if (UpgradeManager.Instance != null && !isMaxLevel)
            {
                Hero hero = FindObjectOfType<Hero>();
                if (hero != null)
                {
                    purchaseButton.interactable = hero.balance >= upgradeData.GetCurrentCost();
                }
            }
        }
    }
    
    private string FormatUpgradeValue(UpgradeManager.UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeManager.UpgradeType.MovementSpeed:
                return $"+{value * 100}% скорости";
                
            case UpgradeManager.UpgradeType.InventoryCapacity:
                return $"+{(int)value} слотов";
                
            case UpgradeManager.UpgradeType.ProductDiscount:
                return $"{value * 100}% скидка";
                
            default:
                return value.ToString();
        }
    }
    
    private void OnPurchaseButtonClicked()
    {
        if (UpgradeManager.Instance != null)
        {
            if (UpgradeManager.Instance.PurchaseUpgrade(upgradeType))
            {
                UpdateUI();
            }
        }
    }
}