using TMPro;
using UnityEngine;

public class HeroInteraction : MonoBehaviour
{
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private Transform hero;
    [SerializeField] private float interactionDistance = 2f;
    public GameObject invWindow;
    public TextMeshProUGUI moneyText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Vector2.Distance(transform.position, hero.position) <= interactionDistance)
        {
            queueManager.HandleHeroInteraction();
            SellRequestedItem();
        }
        else { return; }
    }
    
    private void SellRequestedItem()
    {
        Hero playerHero = hero.gameObject.GetComponent<Hero>();
        Inventory playerInventory = hero.gameObject.GetComponent<Inventory>();
        
        // Получаем текущего покупателя и его запрос
        Customer currentCustomer = queueManager.GetCurrentCustomer();
        if (currentCustomer == null) return;
        
        Item.ItemType requestedItemType = currentCustomer.RequestedItem;
        bool itemFound = false;
        
        for (int i = 0; i < playerInventory.Items.Count; i++)
        {
            if (playerInventory.Items[i].Item.ItemT == requestedItemType)
            {
                // Нашли запрошенный товар
                itemFound = true;
                
                // Добавляем стоимость товара к балансу игрока
                int itemCost = playerInventory.Items[i].Item.Cost;
                playerHero.AddMoney(itemCost);
                
                // Обновляем значение инвентаря
                playerInventory.ValInvetory -= itemCost;
                
                // Уменьшаем количество товара в инвентаре
                if (playerInventory.Items[i].Count > 1)
                {
                    // Если товаров больше одного, уменьшаем количество
                    Inventory.ItemsList updatedItem = new Inventory.ItemsList(
                        playerInventory.Items[i].Item,
                        playerInventory.Items[i].Count - 1
                    );
                    playerInventory.Items[i] = updatedItem;
                    
                    // Обновляем отображение количества в UI инвентаря
                    GameObject slot = invWindow.transform.GetChild(i).gameObject;
                    TextMeshProUGUI countText = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (countText != null)
                    {
                        countText.text = updatedItem.Count.ToString();
                    }
                }
                else
                {
                    // Если это последний товар, удаляем его из инвентаря
                    playerInventory.Items.RemoveAt(i);
                    
                    // Удаляем слот из UI инвентаря
                    Destroy(invWindow.transform.GetChild(i).gameObject);
                }
                
                break;
            }
        }
        
        // Если товар не найден, выводим сообщение
        if (!itemFound)
        {
            Debug.Log($"У игрока нет запрошенного товара: {requestedItemType}");
            // Здесь можно добавить логику негативной реакции покупателя
        }
        
        moneyText.text = playerHero.balance.ToString();
    }
}
