using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<ItemsList> Items = new List<ItemsList>();
}