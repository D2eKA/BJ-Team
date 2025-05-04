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
    [SerializeField] private SpriteRenderer box_sprite;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Shelf shelf;
    private GameObject clone;
    [SerializeField] private Sprite active;
    [SerializeField] private Sprite unactive;
    [SerializeField] private GameObject slot_pref;
    [SerializeField] private AudioSource takeSound;
    
    private void Awake()
    {
        shelf = GetComponentInParent<Shelf>();
        inventory = Inventory.Instance;
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

                // Проверяем, есть ли место в инвентаре
                if (!inventory.CanAddItems(1))
                {
                    Debug.Log("Инвентарь заполнен!");
                    // Здесь можно добавить звуковой сигнал или визуальное оповещение
                    return;
                }

                shelf.count--;
                shelf.UpdateCountDisplay();
                
                takeSound?.Play();

                for (int i = 0; i < inventory.Items.Count; i++)
                {
                    if (inventory.Items[i].Item.ItemT == shelf.item)
                    {
                        // Увеличиваем количество предметов
                        inventory.Items[i] =
                            new Inventory.ItemsList(inventory.Items[i].Item, inventory.Items[i].Count + 1);
                        UpdateSlot(inventory.container.GetChild(i).gameObject, inventory.Items[i].Count);
                        inventory.ValInvetory += shelf.product.Cost;
                        inventory.UpdateCapacityDisplay(); // Обновляем отображение
                        return;
                    }
                }

                inventory.Items.Add(new Inventory.ItemsList(shelf.product, 1));
                CreateSlot(shelf.sprite);
                inventory.ValInvetory += shelf.product.Cost;
                inventory.UpdateCapacityDisplay(); // Обновляем отображение
                Debug.Log("Play");
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
        GameObject slot = Instantiate(slot_pref, inventory.container);
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
