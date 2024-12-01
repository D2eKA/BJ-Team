using System.Collections;
using System.Collections.Generic;
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
        public int Cost;
        public Product(ItemType item)
        {
            Name = item.ToString();
            ItemT = item;
            switch(item)
            {
                    case ItemType.Potato:
                    Cost = 5; break;
                    case ItemType.Cucumber:
                    Cost = 10; break;
                    case ItemType.Tomato:
                    Cost = 15; break;
                    case ItemType.Eggplant:
                    Cost = 20; break;
                    case ItemType.Corn:
                    Cost = 25; break;
                    default:
                    Cost = 0; break;
            }
        }
    }
}
