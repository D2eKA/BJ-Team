using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public struct ItemsList
    {
        [HideInInspector] public string Name;
        public Item.ItemType Item;
        public int Count;
        public ItemsList(Item.ItemType item, int count)
        {
            Item = item;
            Count = count;
            Name = item.ToString();
        }
    }
    public List<ItemsList> Items = new List<ItemsList>();
}