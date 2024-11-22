using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float interactionRadius = 1f; // Радиус взаимодействия
    [SerializeField] private LayerMask interactableLayer;  // Слой для объектов, с которыми можно взаимодействовать
    private int maxInventorySize = 10;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private List<string> inventory = new List<string>(); // Инвентарь персонажа

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        State = States.idle;

        // Передвижение
        if (Input.GetButton("Horizontal"))
            HorizontalMove();
        if (Input.GetButton("Vertical"))
            VerticalMove();

        // Взаимодействие с объектом
        if (Input.GetKeyDown(KeyCode.E))
            Interact();
    }

    private void HorizontalMove()
    {
        State = States.move;
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void VerticalMove()
    {
        State = States.move;
        Vector3 dir = transform.up * Input.GetAxis("Vertical");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

    public void AddToInventory(string itemName)
    {
        Debug.Log($"Попытка добавить предмет '{itemName}' в инвентарь.");

        if (inventory.Count < maxInventorySize)
        {
            inventory.Add(itemName);
            Debug.Log($"Предмет '{itemName}' добавлен в инвентарь. Текущий размер инвентаря: {inventory.Count}");
        }
        else
        {
            Debug.Log("Инвентарь полный, не могу добавить предмет.");
        }
    }

    private void Interact()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);

        if (hitColliders.Length > 0)
        {
            Debug.Log("Объект для взаимодействия найден.");

            // Проверяем, есть ли на объекте интерфейс IInteractable
            IInteractable interactable = hitColliders[0].GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("Объект реализует IInteractable. Выполняем взаимодействие.");
                interactable.OnInteract();
            }
            else
            {
                Debug.Log("Объект найден, но не реализует IInteractable.");
            }
        }
        else
        {
            Debug.Log("Нет объектов для взаимодействия в радиусе.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

public enum States
{
    idle,
    move
}



public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName; // Название предмета

    public void OnInteract()
    {
        Debug.Log($"Предмет подобран: {itemName}");
    }

    public string GetItemName()
    {
        return itemName;
    }
}