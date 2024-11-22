using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class take_item : MonoBehaviour
{
    public inventar inventory;

    void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.gameObject.tag == "Покупатель")
            {
                return;
            }
            else if (inventory.Items.Count > 0)
            {

                for (int i = 0; i < inventory.Items.Count; i++)
                {

                    if (inventory.Items[i].ItemName == other.gameObject.tag)
                    {

                        // Увеличиваем количество предметов
                        inventory.Items[i] = new inventar.ItemsList(inventory.Items[i].ItemName, inventory.Items[i].Count + 1);
                        return;
                    }

                }
                inventory.Items.Add(new inventar.ItemsList(other.gameObject.tag, 1));
            }
            else
            {
                inventory.Items.Add(new inventar.ItemsList(other.gameObject.tag, 1));
            }
        }

    }
}