using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image productIcon;
    [SerializeField] private TextMeshProUGUI productNameText;
    [SerializeField] private TextMeshProUGUI productPriceText;
    [SerializeField] private TMP_InputField quantityInputField;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    private Item.ItemType productType;
    private ShopManager shopManager;
    private int currentQuantity = 1;
    private int maxQuantity = 99;

    private void Start()
    {
        shopManager = FindObjectOfType<ShopManager>();
        
        if (quantityInputField != null)
        {
            quantityInputField.text = currentQuantity.ToString();
            quantityInputField.onEndEdit.AddListener(OnQuantityChanged);
        }
        
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
        
        if (increaseButton != null)
        {
            increaseButton.onClick.AddListener(IncreaseQuantity);
        }
        
        if (decreaseButton != null)
        {
            decreaseButton.onClick.AddListener(DecreaseQuantity);
        }
    }

    public void Initialize(Item.ItemType type)
    {
        productType = type;
    
        if (ProductManager.Instance != null)
        {
            string name = ProductManager.Instance.GetProductName(type);
            int price = ProductManager.Instance.GetProductBuyingPrice(type); // Используем цену покупки
            Sprite icon = ProductManager.Instance.GetProductSprite(type);
        
            if (icon != null && productIcon != null)
            {
                try
                {
                    var aspectFitter = productIcon.GetComponent<AspectRatioFitter>();
                    if (aspectFitter != null)
                    {
                        float newAspectRatio = icon.rect.width / icon.rect.height;
                        aspectFitter.aspectRatio = newAspectRatio;
                    }
                }
                catch (System.NullReferenceException)
                {
                    Debug.LogWarning("Проблема с установкой пропорций для иконки товара");
                }
            }

            if (productNameText != null)
                productNameText.text = name;
            
            if (productPriceText != null)
                productPriceText.text = price.ToString() + " монет";
            
            if (productIcon != null && icon != null)
                productIcon.sprite = icon;
        }
    }

    private void OnBuyButtonClicked()
    {
        if (shopManager != null)
        {
            shopManager.BuyProduct(productType, currentQuantity);
        }
    }
    
    private void OnQuantityChanged(string value)
    {
        if (int.TryParse(value, out int quantity))
        {
            currentQuantity = Mathf.Clamp(quantity, 1, maxQuantity);
        }
        else
        {
            currentQuantity = 1;
        }
        
        quantityInputField.text = currentQuantity.ToString();
    }
    
    private void IncreaseQuantity()
    {
        currentQuantity = Mathf.Min(currentQuantity + 1, maxQuantity);
        quantityInputField.text = currentQuantity.ToString();
    }
    
    private void DecreaseQuantity()
    {
        currentQuantity = Mathf.Max(currentQuantity - 1, 1);
        quantityInputField.text = currentQuantity.ToString();
    }
}
