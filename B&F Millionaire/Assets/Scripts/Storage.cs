using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class Storage : MonoBehaviour
{
    [SerializeField] GameObject storage;
    public List<ItemsList> Items = new();
    public int count;
    public Item.Product child_prod;
    private void Awake()
    {
        Shelf child;
        for (int i = 0; i < storage.transform.childCount; i++)
        {
            child = storage.transform.GetChild(i).transform.gameObject.GetComponent<Shelf>();
            child_prod = child.product;
            Items.Add(new ItemsList(child.product, child.count));
            count += child.GetComponent<Shelf>().count;
        }
    }
    public void UpdateStorage()
    {

    }
}
