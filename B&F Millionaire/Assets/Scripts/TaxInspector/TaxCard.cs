using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaxCard : MonoBehaviour
{
    [SerializeField] private Image cardBack;
    [SerializeField] private Image cardFront; // Теперь необязательный компонент
    [SerializeField] private TextMeshProUGUI cardText;
    [SerializeField] private Button cardButton;
    
    [Header("Спрайты")]
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private Sprite returnCardSprite;
    [SerializeField] private Sprite penaltyCardSprite;
    [SerializeField] private Sprite auditCardSprite;
    
    private TaxCardType cardType;
    private TaxInspectorGame gameManager;
    private bool isRevealed = false;
    
    public void Initialize(TaxCardType type, TaxInspectorGame manager)
    {
        cardType = type;
        gameManager = manager;
        isRevealed = false;
        
        // Настраиваем внешний вид закрытой карты
        if (cardBack != null)
            cardBack.sprite = cardBackSprite;
            
        if (cardBack != null)
            cardBack.gameObject.SetActive(true);
            
        // Проверяем наличие лицевой стороны карты
        if (cardFront != null)
            cardFront.gameObject.SetActive(false);
        
        // Настраиваем кнопку
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
            cardButton.interactable = true;
        }
    }
    
    public void OnCardClicked()
    {
        if (isRevealed)
            return;
            
        RevealCard();
        
        // Уведомляем менеджер игры об открытии карты
        if (gameManager != null)
            gameManager.OnCardOpened(cardType);
    }
    
    public void RevealCard()
    {
        isRevealed = true;
        
        // Показываем лицевую сторону карты
        if (cardBack != null)
            cardBack.gameObject.SetActive(false);
            
        // Проверяем наличие лицевой стороны
        if (cardFront != null)
            cardFront.gameObject.SetActive(true);
        
        // Настраиваем внешний вид и текст в зависимости от типа карты
        switch (cardType)
        {
            case TaxCardType.ReturnSmall:
                if (cardFront != null)
                    cardFront.sprite = returnCardSprite;
                if (cardText != null)
                {
                    cardText.text = "+50%";
                    cardText.color = Color.green;
                }
                break;
                
            case TaxCardType.ReturnMedium:
                if (cardFront != null)
                    cardFront.sprite = returnCardSprite;
                if (cardText != null)
                {
                    cardText.text = "+100%";
                    cardText.color = Color.green;
                }
                break;
                
            case TaxCardType.ReturnLarge:
                if (cardFront != null)
                    cardFront.sprite = returnCardSprite;
                if (cardText != null)
                {
                    cardText.text = "+200%";
                    cardText.color = Color.green;
                }
                break;
                
            case TaxCardType.PenaltySmall:
                if (cardFront != null)
                    cardFront.sprite = penaltyCardSprite;
                if (cardText != null)
                {
                    cardText.text = "-50%";
                    cardText.color = Color.red;
                }
                break;
                
            case TaxCardType.PenaltyLarge:
                if (cardFront != null)
                    cardFront.sprite = penaltyCardSprite;
                if (cardText != null)
                {
                    cardText.text = "-100%";
                    cardText.color = Color.red;
                }
                break;
                
            case TaxCardType.Audit:
                if (cardFront != null)
                    cardFront.sprite = auditCardSprite;
                if (cardText != null)
                {
                    cardText.text = "АУДИТ";
                    cardText.color = Color.red;
                }
                break;
        }
        
        // Отключаем кнопку
        if (cardButton != null)
            cardButton.interactable = false;
    }
    
    public void SetInteractable(bool interactable)
    {
        if (cardButton != null)
            cardButton.interactable = interactable;
    }
}
