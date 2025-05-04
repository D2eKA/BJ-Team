using UnityEngine;

public class Item : MonoBehaviour
{
    [System.Serializable]
    public enum ItemType
    {
        None,
        Potato,
        Cucumber,
        Tomato,
        Eggplant,
        Corn
    }
    
    public struct Product
    {
        public string Name;
        public ItemType ItemT;
        public int Cost; // Стоимость продажи
        
        public Product(ItemType item)
        {
            Name = item.ToString();
            ItemT = item;
            
            Cost = ProductManager.Instance.GetProductSellingPrice(item);
        }
    }
}