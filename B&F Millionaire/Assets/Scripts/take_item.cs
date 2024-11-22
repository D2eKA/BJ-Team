using UnityEngine;




public class Storage : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Яблоко"; // Название предмета
    [SerializeField] private int itemCount = 1; // Количество предметов

    private bool isPlayerNearby = false;
    private Hero player;

    public void OnInteract()
    {
        if (isPlayerNearby && player != null)
        {
            Debug.Log($"Начинаем добавление предмета '{itemName}' в инвентарь.");

            // Добавляем предмет в инвентарь
            for (int i = 0; i < itemCount; i++)
            {
                player.AddToInventory(itemName);
            }
            Debug.Log($"Добавлено {itemCount} '{itemName}' в инвентарь.");

            // После того как предмет добавлен, можно уничтожить объект (если необходимо)
            // Destroy(gameObject);
        }
        else
        {
            Debug.Log("Невозможно взаимодействовать с объектом.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = collision.GetComponent<Hero>();
            Debug.Log("Персонаж вошел в радиус взаимодействия с объектом.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
            Debug.Log("Персонаж покинул радиус взаимодействия с объектом.");
        }
    }
}
