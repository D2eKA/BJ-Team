using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public struct ItemsList
    {
        public string Name;
        public Item.Product Item;
        public int Count;
        public ItemsList(Item.Product item, int count)
        {
            Name = item.Name;
            Item = item;
            Count = count;
        }
    }
    
    [SerializeField] private int maxCapacity = 50; // Максимальная вместимость инвентаря
    [SerializeField] private TextMeshProUGUI capacityText; // Текст для отображения заполненности
    
    public List<ItemsList> Items = new List<ItemsList>();
    public int ValInvetory = 0;
    
    private void Start()
    {
        UpdateCapacityDisplay();
    }
    
    // Получаем текущее количество предметов
    public int CurrentItemCount 
    {
        get
        {
            int count = 0;
            foreach (ItemsList item in Items)
            {
                count += item.Count;
            }
            return count;
        }
    }
    
    // Проверка возможности добавления предметов
    public bool CanAddItems(int amount)
    {
        return CurrentItemCount + amount <= maxCapacity;
    }
    
    // Обновление отображения заполненности инвентаря
    public void UpdateCapacityDisplay()
    {
        if (capacityText != null)
        {
            capacityText.text = $"Предметы: {CurrentItemCount}/{maxCapacity}";
            
            // Меняем цвет в зависимости от заполненности
            float fillRatio = (float)CurrentItemCount / maxCapacity;
            if (fillRatio > 0.9f)
                capacityText.color = Color.red;
            else if (fillRatio > 0.7f)
                capacityText.color = new Color(1.0f, 0.7f, 0.0f); // Оранжевый
            else
                capacityText.color = Color.white;
        }
    }
}