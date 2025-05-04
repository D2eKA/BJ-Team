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
        else
        {
            return;
        }
    }

    private void SellRequestedItem()
    {
        Hero playerHero = hero.gameObject.GetComponent<Hero>();
        Inventory playerInventory = hero.gameObject.GetComponent<Inventory>();

        // Получаем текущего покупателя и его запрос
        Customer currentCustomer = queueManager.GetCurrentCustomer();
        if (currentCustomer == null) return;

        Item.ItemType requestedItemType = currentCustomer.RequestedItem;
        int requestedQuantity = currentCustomer.RequestedQuantity;
        bool hasEnoughItems = false;

        for (int i = 0; i < playerInventory.Items.Count; i++)
        {
            if (playerInventory.Items[i].Item.ItemT == requestedItemType)
            {
                // Проверяем, достаточно ли у игрока товаров
                if (playerInventory.Items[i].Count >= requestedQuantity)
                {
                    hasEnoughItems = true;

                    // Добавляем стоимость товаров к балансу игрока
                    int itemCost = playerInventory.Items[i].Item.Cost * requestedQuantity;
                    playerHero.AddMoney(itemCost);

                    // Обновляем значение инвентаря
                    playerInventory.ValInvetory -= playerInventory.Items[i].Item.Cost * requestedQuantity;

                    // Уменьшаем количество товара в инвентаре
                    Inventory.ItemsList updatedItem = new Inventory.ItemsList(
                        playerInventory.Items[i].Item,
                        playerInventory.Items[i].Count - requestedQuantity
                    );

                    if (updatedItem.Count <= 0)
                    {
                        // Если товаров не осталось, удаляем их из инвентаря
                        playerInventory.Items.RemoveAt(i);

                        // Удаляем слот из UI инвентаря
                        Destroy(invWindow.transform.GetChild(i).gameObject);
                    }
                    else
                    {
                        playerInventory.Items[i] = updatedItem;

                        // Обновляем отображение количества в UI инвентаря
                        GameObject slot = invWindow.transform.GetChild(i).gameObject;
                        TextMeshProUGUI countText = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        if (countText != null)
                        {
                            countText.text = updatedItem.Count.ToString();
                        }
                    }

                    // Обновляем отображение ограничения инвентаря
                    if (playerInventory.GetType().GetMethod("UpdateCapacityDisplay") != null)
                    {
                        playerInventory.UpdateCapacityDisplay();
                    }
                }

                break;
            }
        }

        // Если у игрока нет достаточного количества товаров, выводим сообщение
        if (!hasEnoughItems)
        {
            Debug.Log($"У игрока недостаточно товаров: {requestedItemType} x{requestedQuantity}");
            // Здесь можно добавить логику негативной реакции покупателя
        }

        moneyText.text = playerHero.balance.ToString();
    }
}
