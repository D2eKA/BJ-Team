using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Interact : MonoBehaviour
{
    public bool heroInRange;
    [SerializeField] SpriteRenderer box_sprite;
    [SerializeField] Inventory inventory;
    [SerializeField] Shelf shelf;
    private GameObject clone;
    [SerializeField] Sprite active;
    [SerializeField] Sprite unactive;
    [SerializeField] GameObject slot_pref;
    private GameObject slot_grid;
    private void Awake()
    {
        shelf = GetComponentInParent<Shelf>();
        slot_grid = GameObject.Find("InventoryWindow");
    }
    private void Update()
    {
        if (heroInRange)
        {
            box_sprite.sprite = active;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (shelf.item == Item.ItemType.None | shelf.count == 0)
                    return;
                shelf.count--;
                shelf.UpdateCountDisplay();
                for (int i = 0; i < inventory.Items.Count; i++)
                {
                    if (inventory.Items[i].Item.ItemT == shelf.item)
                    {
                        // Увеличиваем количество предметов
                        inventory.Items[i] = new Inventory.ItemsList(inventory.Items[i].Item, inventory.Items[i].Count + 1);
                        UpdateSlot(slot_grid.transform.GetChild(i).gameObject, inventory.Items[i].Count);
                        inventory.ValInvetory += shelf.product.Cost;
                        return;
                    }
                }
                inventory.Items.Add(new Inventory.ItemsList(shelf.product, 1));
                CreateSlot(shelf.sprite);
                inventory.ValInvetory += shelf.product.Cost;
            }
        }
        if (!heroInRange)
        {
            box_sprite.sprite = unactive;
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
    private void CreateSlot(Sprite sprite)
    {
        GameObject slot = Instantiate(slot_pref, GameObject.Find("InventoryWindow").transform);
        slot.name = shelf.item.ToString();
        UnityEngine.UI.Image icon = slot.transform.GetChild(0).gameObject.GetComponentAtIndex<UnityEngine.UI.Image>(2);
        icon.sprite = sprite;
    }
    private void UpdateSlot(GameObject slot, int count)
    {
        TextMeshProUGUI text = slot.transform.GetChild(1).gameObject.GetComponentAtIndex<TextMeshProUGUI>(2);
        text.text = count.ToString();
    }
}
