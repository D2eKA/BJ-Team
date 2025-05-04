using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaxInspectorGame : MonoBehaviour
{
    [Header("Основные компоненты")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField betInputField;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI winningsText;
    [SerializeField] private TextMeshProUGUI messageText; // Отдельный текст для сообщений
    [SerializeField] private Slider betSlider;
    [SerializeField] private TextMeshProUGUI betPercentText;

    [Header("Настройки игры")]
    [SerializeField] private int numberOfCards = 6;
    [SerializeField] private float minBetPercent = 0.1f;
    [SerializeField] private float maxBetPercent = 0.3f;
    [SerializeField] private int maxCardsToOpen = 3;

    private Hero playerHero;
    private int currentBet;
    private int currentWinnings;
    private int cardsOpened;
    private bool gameInProgress;
    private List<TaxCard> cards = new List<TaxCard>();
    
    // Добавляем отслеживание корутин и идентификатор сессии
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private int gameSession = 0;

    public static TaxInspectorGame Instance { get; private set; }

    private void Awake()
    {
        // Синглтон
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
        // Находим игрока
        playerHero = FindObjectOfType<Hero>();
        // Настраиваем UI
        if (gamePanel != null)
            gamePanel.SetActive(false);
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseGame);
        if (betSlider != null)
        {
            betSlider.minValue = minBetPercent * 100;
            betSlider.maxValue = maxBetPercent * 100;
            betSlider.value = minBetPercent * 100;
            betSlider.onValueChanged.AddListener(OnBetSliderChanged);
        }
        
        // Скрываем текст сообщений в начале
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    // Метод для остановки всех активных корутин
    private void StopAllActiveCoroutines()
    {
        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
        // Добавляем вызов StopAllCoroutines() для гарантированной остановки всех корутин
        StopAllCoroutines();
    }
    
    // Вспомогательный метод для запуска и отслеживания корутин
    private Coroutine StartAndTrackCoroutine(IEnumerator routine)
    {
        Coroutine coroutine = StartCoroutine(routine);
        activeCoroutines.Add(coroutine);
        return coroutine;
    }

    // Открытие панели мини-игры
    public void OpenGame()
    {
        if (gamePanel == null || playerHero == null)
            return;
        gamePanel.SetActive(true);
        UpdatePlayerMoneyText();
        ResetGame();
        // Настраиваем начальную ставку
        OnBetSliderChanged(betSlider.value);
    }

    // Закрытие панели мини-игры
    public void CloseGame()
    {
        if (gamePanel == null)
            return;
        if (gameInProgress && currentWinnings > 0)
        {
            EndGame(true);
        }
        gamePanel.SetActive(false);
        gameInProgress = false;
        // Уведомляем QueueManager о завершении мини-игры
        QueueManager queueManager = FindObjectOfType<QueueManager>();
        if (queueManager != null)
        {
            queueManager.ContinueAfterMinigame();
        }
    }

    // Обработка изменения ползунка ставки
    private void OnBetSliderChanged(float value)
    {
        if (playerHero == null)
            return;
        float percentage = value / 100f;
        int maxBet = Mathf.FloorToInt(playerHero.balance * maxBetPercent);
        int minBet = Mathf.Max(10, Mathf.FloorToInt(playerHero.balance * minBetPercent));
        currentBet = Mathf.Clamp(Mathf.FloorToInt(playerHero.balance * percentage), minBet, maxBet);
        if (betInputField != null)
            betInputField.text = currentBet.ToString();
        if (betPercentText != null)
            betPercentText.text = $"{value:F0}%";
    }

    // Начало игры
    private void StartGame()
    {
        if (playerHero == null || cardsContainer == null || gameInProgress)
            return;
        // Проверяем, достаточно ли денег у игрока
        if (playerHero.balance < currentBet)
            return;

        // Останавливаем любые активные корутины из предыдущей игры
        StopAllActiveCoroutines();
        
        // Увеличиваем идентификатор игровой сессии
        gameSession++;
        
        // Скрываем сообщения
        if (messageText != null)
            messageText.gameObject.SetActive(false);
            
        // Списываем ставку
        playerHero.AddMoney(-currentBet);
        UpdatePlayerMoneyText();
        // Сбрасываем состояние игры
        currentWinnings = 0;
        cardsOpened = 0;
        gameInProgress = true;
        // Создаем карты
        CreateCards();
        // Обновляем UI
        startGameButton.gameObject.SetActive(false);
        betSlider.gameObject.SetActive(false);
        betInputField.gameObject.SetActive(false);
        winningsText.gameObject.SetActive(true);
        winningsText.text = $"Выигрыш: 0";
    }

    // Создание карт
    private void CreateCards()
    {
        // Удаляем существующие карты
        foreach (Transform child in cardsContainer)
            Destroy(child.gameObject);
        cards.Clear();
        // Создаем список типов карт
        List<TaxCardType> cardTypes = new List<TaxCardType>();
        // Добавляем типы карт с учетом шансов
        // 60% проигрыш (штрафы и аудит)
        // 40% выигрыш (возвраты)
        cardTypes.Add(TaxCardType.ReturnSmall); // +50%
        cardTypes.Add(TaxCardType.ReturnMedium); // +100%
        cardTypes.Add(TaxCardType.ReturnLarge); // +200%
        cardTypes.Add(TaxCardType.PenaltySmall); // -50%
        cardTypes.Add(TaxCardType.PenaltyLarge); // -100%
        cardTypes.Add(TaxCardType.Audit); // -50% от общего выигрыша
        // Перемешиваем типы карт
        for (int i = 0; i < cardTypes.Count; i++)
        {
            int randomIndex = Random.Range(i, cardTypes.Count);
            TaxCardType temp = cardTypes[i];
            cardTypes[i] = cardTypes[randomIndex];
            cardTypes[randomIndex] = temp;
        }
        // Создаем карты и размещаем их в контейнере
        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardsContainer);
            TaxCard card = cardObject.GetComponent<TaxCard>();
            if (card != null)
            {
                card.Initialize(cardTypes[i % cardTypes.Count], this);
                cards.Add(card);
            }
        }
    }

    // Метод для блокировки всех карт
    private void DisableAllCards()
    {
        foreach (TaxCard card in cards)
        {
            if (card != null)
                card.SetInteractable(false);
        }
    }

    // Обработка результата открытия карты
    public void OnCardOpened(TaxCardType cardType)
    {
        // Если игра уже не активна, игнорируем открытие карты
        if (!gameInProgress)
            return;
            
        cardsOpened++;
        // Рассчитываем результат в зависимости от типа карты
        switch (cardType)
        {
            case TaxCardType.ReturnSmall:
                currentWinnings += Mathf.RoundToInt(currentBet * 0.5f);
                break;
            case TaxCardType.ReturnMedium:
                currentWinnings += Mathf.RoundToInt(currentBet * 1.0f);
                break;
            case TaxCardType.ReturnLarge:
                currentWinnings += Mathf.RoundToInt(currentBet * 2.0f);
                break;
            case TaxCardType.PenaltySmall:
                currentWinnings -= Mathf.RoundToInt(currentBet * 0.5f);
                break;
            case TaxCardType.PenaltyLarge:
                currentWinnings -= Mathf.RoundToInt(currentBet * 1.0f);
                break;
            case TaxCardType.Audit:
                // При выпадении аудита немедленно блокируем все карты
                DisableAllCards();
                currentWinnings = Mathf.RoundToInt(currentWinnings * 0.5f);
                UpdateWinningsText();
                ShowMessage("Налоговый аудит! Ваш выигрыш уменьшен на 50%");
                StartAndTrackCoroutine(DelayedEndGame(2f, gameSession));
                return;
        }
        
        // Обновляем текст выигрыша
        UpdateWinningsText();
        
        // Проверяем условия завершения игры
        CheckGameEndConditions();
    }
    
    // Обновление текста выигрыша
    private void UpdateWinningsText()
    {
        if (winningsText != null)
            winningsText.text = $"Выигрыш: {currentWinnings}";
    }
    
    // Показ сообщения
    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = message;
        }
    }
    
    // Скрытие сообщения
    private void HideMessage()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    // Завершение игры с задержкой
    private IEnumerator DelayedEndGame(float delay, int sessionId)
    {
        yield return new WaitForSeconds(delay);
    
        // Улучшенная проверка актуальности сессии
        if (sessionId != gameSession || !gameInProgress)
        {
            yield break;
        }
    
        HideMessage();
        EndGame(true);
    }


    // Проверка условий завершения игры
    private void CheckGameEndConditions()
    {
        // Если игра уже не активна, выходим
        if (!gameInProgress) return;
        
        // Проверяем только достижение максимального количества карт
        if (cardsOpened >= maxCardsToOpen)
        {
            // Блокируем взаимодействие со всеми картами
            DisableAllCards();
            
            ShowMessage("Достигнуто максимальное количество карт!");
            StartAndTrackCoroutine(DelayedEndGame(2f, gameSession));
        }
    }

    // Завершение игры
    public void EndGame(bool collectWinnings)
    {
        if (!gameInProgress)
            return;
            
        gameInProgress = false;
        
        // Блокируем карты
        DisableAllCards();
        
        // Если нужно собрать выигрыш, добавляем его игроку
        if (collectWinnings && currentWinnings > 0)
        {
            playerHero.AddMoney(currentWinnings);
            UpdatePlayerMoneyText();
        }
        
        // Показываем кнопку для новой игры
        startGameButton.gameObject.SetActive(true);
        betSlider.gameObject.SetActive(true);
        betInputField.gameObject.SetActive(true);
    }

    // Сброс состояния игры
    private void ResetGame()
    {
        // Останавливаем любые активные корутины
        StopAllActiveCoroutines();
        
        currentBet = 0;
        currentWinnings = 0;
        cardsOpened = 0;
        gameInProgress = false;
        
        // Скрываем сообщение
        HideMessage();
        
        // Удаляем существующие карты
        foreach (Transform child in cardsContainer)
            Destroy(child.gameObject);
        cards.Clear();
        // Обновляем UI
        startGameButton.gameObject.SetActive(true);
        betSlider.gameObject.SetActive(true);
        betInputField.gameObject.SetActive(true);
        winningsText.gameObject.SetActive(false);
    }

    // Обновление отображения денег игрока
    private void UpdatePlayerMoneyText()
    {
        if (playerMoneyText != null && playerHero != null)
        {
            playerMoneyText.text = $"Деньги: {playerHero.balance}";
        }
    }
}

// Типы карт
public enum TaxCardType
{
    ReturnSmall, // +50%
    ReturnMedium, // +100%
    ReturnLarge, // +200%
    PenaltySmall, // -50%
    PenaltyLarge, // -100%
    Audit // -50% общего выигрыша
}
