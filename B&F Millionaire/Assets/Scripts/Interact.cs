using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public bool heroInRange;
    private SpriteRenderer sprite;
    [SerializeField] Inventory inventory;
    [SerializeField] Shelf shelf;
    [SerializeField] GameObject empty;
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        shelf = GetComponentInParent<Shelf>();
    }
    private void Update()
    {
        sprite.color = Color.white;
        if (heroInRange)
        {
            sprite.color = Color.red;
            if (heroInRange)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (shelf.item == Item.ItemType.None)
                    {
                        return;
                    }
                    if (shelf.count == 0)
                    {
                        Instantiate(empty, shelf.transform.position, shelf.transform.rotation);
                        Destroy(transform.parent.gameObject);
                        return;
                    }
                    shelf.count--;
                    if (inventory.Items.Count > 0)
                    {
                        for (int i = 0; i < inventory.Items.Count; i++)
                        {

                            if (inventory.Items[i].Item == shelf.item)
                            {
                                // Увеличиваем количество предметов
                                inventory.Items[i] = new Inventory.ItemsList(inventory.Items[i].Item, inventory.Items[i].Count + 1);

                                return;
                            }
                        }
                        inventory.Items.Add(new Inventory.ItemsList(shelf.item, 1));
                    }
                    else
                    {
                        inventory.Items.Add(new Inventory.ItemsList(shelf.item, 1));
                    }
                }
            }
        }
        if (!heroInRange)
        {
            sprite.color = Color.white;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inventory = collision.GetComponent<Inventory>();
            heroInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inventory = null;
            heroInRange = false;
        }
    }
}
