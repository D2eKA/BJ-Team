using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventar : MonoBehaviour
{
    [System.Serializable]
    public struct ItemsList
    {
        public string ItemName;
        public int Count;
        public ItemsList(string itemName, int count)
        {
            ItemName = itemName;
            Count = count;
        }
    }
    public List<ItemsList> Items = new List<ItemsList>();
}