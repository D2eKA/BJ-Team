using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShelfRefiller : MonoBehaviour
{
    [SerializeField] private GameObject refillPromptUI; // UI подсказки
    [SerializeField] private TextMeshProUGUI promptText; // Текст подсказки
    
    private Inventory playerInventory;
    private Shelf interactableShelf;
    private Interact interactComponent;
    
    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerInventory = playerObject.GetComponent<Inventory>();
        }
        
        if (refillPromptUI != null)
        {
            refillPromptUI.SetActive(false);
        }
    }
    
    private void Update()
    {
        FindInteractableShelf();
        
        // Пополняем полку при нажатии R
        if (interactableShelf != null && interactComponent != null && 
            interactComponent.heroInRange && Input.GetKeyDown(KeyCode.R))
        {
            RefillShelf(interactableShelf);
        }
    }
    
    // Находит полку, с которой можно взаимодействовать
    private void FindInteractableShelf()
    {
        interactableShelf = null;
        interactComponent = null;
        
        // Находим все компоненты Interact в сцене
        Interact[] interactObjects = FindObjectsOfType<Interact>();
        
        foreach (Interact interact in interactObjects)
        {
            // Если герой в зоне этого компонента
            if (interact.heroInRange)
            {
                interactComponent = interact;
                interactableShelf = interact.GetComponentInParent<Shelf>();
                break;
            }
        }
        
        // Обновляем UI подсказки
        UpdatePromptUI();
    }
    
    // Обновляет текст подсказки
    private void UpdatePromptUI()
    {
        if (refillPromptUI != null)
        {
            if (interactableShelf != null && interactComponent != null && interactComponent.heroInRange)
            {
                refillPromptUI.SetActive(true);
                
                // Проверяем наличие товаров в инвентаре игрока
                int availableItems = GetPlayerItemCount(interactableShelf.item);
                
                if (promptText != null)
                {
                    if (availableItems > 0)
                    {
                        promptText.text = $"Нажмите R для пополнения полки с {ProductManager.Instance.GetProductName(interactableShelf.item)} (Доступно: {availableItems})";
                    }
                    else
                    {
                        promptText.text = $"Нет товаров типа {ProductManager.Instance.GetProductName(interactableShelf.item)} в инвентаре";
                    }
                }
            }
            else
            {
                refillPromptUI.SetActive(false);
            }
        }
    }
    
    // Получает количество товаров определенного типа в инвентаре игрока
    private int GetPlayerItemCount(Item.ItemType itemType)
    {
        if (playerInventory == null)
            return 0;
            
        foreach (Inventory.ItemsList item in playerInventory.Items)
        {
            if (item.Item.ItemT == itemType)
            {
                return item.Count;
            }
        }
        return 0;
    }
    
    // Пополняет полку товарами из инвентаря игрока
    private void RefillShelf(Shelf shelf)
    {
        if (playerInventory == null)
            return;
            
        for (int i = 0; i < playerInventory.Items.Count; i++)
        {
            if (playerInventory.Items[i].Item.ItemT == shelf.item && playerInventory.Items[i].Count > 0)
            {
                // Пополняем полку максимум на 10 единиц
                int amountToAdd = Mathf.Min(10, playerInventory.Items[i].Count);
                
                // Обновляем инвентарь игрока
                Inventory.ItemsList updatedItem = new Inventory.ItemsList(
                    playerInventory.Items[i].Item,
                    playerInventory.Items[i].Count - amountToAdd
                );
                
                // Обновляем значение инвентаря
                playerInventory.ValInvetory -= playerInventory.Items[i].Item.Cost * amountToAdd;
                
                // Если предметов не осталось, удаляем их из инвентаря
                if (updatedItem.Count <= 0)
                {
                    playerInventory.Items.RemoveAt(i);
                    
                    // Удаляем слот из UI инвентаря
                    GameObject invWindow = GameObject.Find("InventoryWindow");
                    if (invWindow != null && i < invWindow.transform.childCount)
                    {
                        Destroy(invWindow.transform.GetChild(i).gameObject);
                    }
                }
                else
                {
                    playerInventory.Items[i] = updatedItem;
                    
                    // Обновляем отображение количества в UI инвентаря
                    GameObject invWindow = GameObject.Find("InventoryWindow");
                    if (invWindow != null && i < invWindow.transform.childCount)
                    {
                        GameObject slot = invWindow.transform.GetChild(i).gameObject;
                        TextMeshProUGUI countText = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        if (countText != null)
                        {
                            countText.text = updatedItem.Count.ToString();
                        }
                    }
                }
                
                // Добавляем товары на полку
                shelf.AddItems(amountToAdd);
                
                Debug.Log($"Полка пополнена: {shelf.item} +{amountToAdd}");
                
                // Обновляем UI подсказки
                UpdatePromptUI();
                return;
            }
        }
        
        Debug.Log($"В инвентаре нет товаров типа {shelf.item} для пополнения полки.");
    }
}
