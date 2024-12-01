using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public Item.ItemType item;
    public int count;
    public Sprite sprite;
    public Item.Product product;
    private void Awake()
    {
        product = new Item.Product(item);
    }
}
