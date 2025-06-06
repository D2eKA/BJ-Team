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
    private AspectRatioFitter iconAspectRatioFitter;

    private void Start()
    {
        shopManager = ShopManager.Instance;
        
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
        
        // Настраиваем AspectRatioFitter для изображения
        SetupAspectRatioFitter();
    }

    // Метод для настройки компонента AspectRatioFitter
    private void SetupAspectRatioFitter()
    {
        if (productIcon != null)
        {
            // Получаем существующий компонент или добавляем новый
            iconAspectRatioFitter = productIcon.GetComponent<AspectRatioFitter>();
            if (iconAspectRatioFitter == null)
            {
                iconAspectRatioFitter = productIcon.gameObject.AddComponent<AspectRatioFitter>();
                iconAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            }
            
            // Если спрайт уже установлен, сразу настраиваем соотношение сторон
            if (productIcon.sprite != null)
            {
                UpdateAspectRatio(productIcon.sprite);
            }
        }
    }

    public void Initialize(Item.ItemType type)
    {
        productType = type;

        if (ProductManager.Instance != null)
        {
            string name = ProductManager.Instance.GetProductName(type);
            Sprite icon = ProductManager.Instance.GetProductSprite(type);

            if (productNameText != null)
                productNameText.text = name;

            if (productIcon != null && icon != null)
            {
                productIcon.sprite = icon;
                
                // Обновляем соотношение сторон на основе загруженного спрайта
                UpdateAspectRatio(icon);
            }
        }
    }
    
    // Метод для обновления соотношения сторон на основе спрайта
    private void UpdateAspectRatio(Sprite sprite)
    {
        if (iconAspectRatioFitter != null && sprite != null && sprite.rect.height > 0)
        {
            float aspectRatio = sprite.rect.width / sprite.rect.height;
            iconAspectRatioFitter.aspectRatio = aspectRatio;
        }
    }

    public void RefreshPrice()
    {
        if (ProductManager.Instance != null)
        {
            int price = ProductManager.Instance.GetProductBuyingPrice(productType);
            productPriceText.text = price.ToString();
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
