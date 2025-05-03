using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shelf : MonoBehaviour
{
    public Item.ItemType item;
    public int count;
    public Sprite sprite;
    public Item.Product product;
    
    [SerializeField] private TextMeshProUGUI countText; // Добавляем поле для текста количества
    [SerializeField] private GameObject countDisplay; // Родительский объект с текстом

    private void Awake()
    {
        product = new Item.Product(item);
        UpdateCountDisplay();
    }
    
    // Метод для обновления отображения количества
    public void UpdateCountDisplay()
    {
        if (countText != null)
        {
            countText.text = count.ToString();
            
            // Показываем или скрываем счетчик в зависимости от наличия товара
            if (countDisplay != null)
            {
                countDisplay.SetActive(count > 0);
            }
        }
    }
    
    // Метод для добавления товаров на полку
    public void AddItems(int amount)
    {
        count += amount;
        UpdateCountDisplay();
    }
}