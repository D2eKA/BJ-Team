using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroInteraction : MonoBehaviour
{
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private Transform hero;
    [SerializeField] private float interactionDistance = 2f;
    public GameObject invWindow;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        // Если переменная hero не назначена, ищем игрока автоматически
        if (hero == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                hero = playerObj.transform;
                Debug.Log("Автоматически найден игрок и назначен переменной hero");
            }
            else
            {
                Debug.LogError("Не удалось найти игрока! Убедитесь, что у объекта игрока есть тег 'Player'");
            }
        }
        
        // Если queueManager не назначен, ищем его
        if (queueManager == null)
        {
            queueManager = FindObjectOfType<QueueManager>();
            if (queueManager == null)
                Debug.LogError("QueueManager не найден!");
        }
        
        // Находим инвентарь, если он не назначен
        if (invWindow == null)
        {
            invWindow = GameObject.Find("InventoryWindow");
        }
    }

    private void Update()
    {
        // Проверка на null перед использованием переменной hero
        if (hero == null)
        {
            Debug.LogError("Переменная hero не назначена! Назначьте её в инспекторе или добавьте тег 'Player' игроку.");
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.E) && Vector2.Distance(transform.position, hero.position) <= interactionDistance)
        {
            // Проверка на queueManager
            if (queueManager == null)
            {
                Debug.LogError("QueueManager не назначен!");
                return;
            }
            
            // Сначала обрабатываем продажу текущему клиенту
            SellRequestedItem();
        
            // Затем перемещаем очередь
            queueManager.HandleHeroInteraction();
        }
    }

    private void SellRequestedItem()
    {
        if (hero == null || queueManager == null)
        {
            Debug.LogError("hero или queueManager не назначены!");
            return;
        }

        Hero playerHero = hero.GetComponent<Hero>();
        Inventory playerInventory = hero.GetComponent<Inventory>();

        if (playerHero == null || playerInventory == null)
        {
            Debug.LogError("На объекте hero отсутствуют компоненты Hero или Inventory!");
            return;
        }

        // Получаем текущего покупателя и его запрос
        Customer currentCustomer = queueManager.GetCurrentCustomer();
        if (currentCustomer == null)
        {
            Debug.Log("Нет покупателя для взаимодействия");
            return;
        }

        Item.ItemType requestedItemType = currentCustomer.RequestedItem;

        // Проверка на случай, если товар не был назначен
        if (requestedItemType == Item.ItemType.None)
        {
            Debug.LogWarning("Клиент запросил товар типа None. Переназначаем товар.");

            // Переназначаем товар, если он не был назначен
            if (ProductManager.Instance != null)
            {
                List<Item.ItemType> availableTypes = ProductManager.Instance.GetAllProductTypes();
                availableTypes.RemoveAll(type => type == Item.ItemType.None);

                if (availableTypes.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);
                    currentCustomer.RequestedItem = availableTypes[randomIndex];
                    requestedItemType = currentCustomer.RequestedItem;
                    Debug.Log($"Новый назначенный товар: {requestedItemType}");
                }
                else
                {
                    Debug.LogError("Нет доступных типов товаров!");
                    return;
                }
            }
            else
            {
                Debug.LogError("ProductManager.Instance равен null!");
                return;
            }
        }

        int requestedQuantity = currentCustomer.RequestedQuantity;

        // Проверяем, есть ли нужный товар в инвентаре
        bool hasEnoughItems = false;
        int itemIndex = -1;

        for (int i = 0; i < playerInventory.Items.Count; i++)
        {
            if (playerInventory.Items[i].Item.ItemT == requestedItemType)
            {
                if (playerInventory.Items[i].Count >= requestedQuantity)
                {
                    hasEnoughItems = true;
                    itemIndex = i;
                }
                break;
            }
        }

        // Если достаточно товаров для продажи
        if (hasEnoughItems)
        {
            // Вычисляем стоимость продажи
            int itemPrice = ProductManager.Instance.GetProductSellingPrice(requestedItemType);
            int totalPrice = itemPrice * requestedQuantity;

            // Обновляем инвентарь игрока
            Inventory.ItemsList updatedItem = new Inventory.ItemsList(
                playerInventory.Items[itemIndex].Item,
                playerInventory.Items[itemIndex].Count - requestedQuantity
            );

            // Если товаров не осталось, удаляем их из инвентаря
            if (updatedItem.Count <= 0)
            {
                playerInventory.Items.RemoveAt(itemIndex);

                // Удаляем слот из UI инвентаря
                if (invWindow != null)
                {
                    if (itemIndex < invWindow.transform.childCount)
                    {
                        Destroy(invWindow.transform.GetChild(itemIndex).gameObject);
                    }
                }
            }
            else
            {
                playerInventory.Items[itemIndex] = updatedItem;

                // Обновляем отображение количества в UI инвентаря
                if (invWindow != null)
                {
                    if (itemIndex < invWindow.transform.childCount)
                    {
                        GameObject slot = invWindow.transform.GetChild(itemIndex).gameObject;
                        TMPro.TextMeshProUGUI countText =
                            slot.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
                        if (countText != null)
                        {
                            countText.text = updatedItem.Count.ToString();
                        }
                    }
                }
            }

            // Добавляем деньги игроку
            playerHero.AddMoney(totalPrice);

            // Обновляем отображение вместимости инвентаря
            playerInventory.UpdateCapacityDisplay();

            Debug.Log($"Продано: {requestedItemType} x{requestedQuantity} за {totalPrice} монет");
        }
        else
        {
            Debug.Log($"Недостаточно товаров для продажи: {requestedItemType} x{requestedQuantity}");
        }
    }
}
