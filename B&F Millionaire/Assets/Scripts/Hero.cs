using UnityEngine;
using TMPro;

public class Hero : MonoBehaviour
{
    public int balance = 0;
    public TextMeshProUGUI moneyText;

    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private void Start()
    {
        UpdateMoneyDisplay();
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        moneyText.text = balance.ToString();
    }

    private States State
    {
        get => (States)anim.GetInteger("state");
        set => anim.SetInteger("state", (int)value);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Получаем ввод и нормализуем вектор направления
        Vector2 direction = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Применяем движение
        rb.velocity = direction * speed;

        // Управление анимацией и поворотом
        if (direction.magnitude > 0.1f)
        {
            State = States.move;
            if (direction.x != 0)
            {
                sprite.flipX = direction.x < 0;
            }
        }
        else
        {
            State = States.idle;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            State = States.interact;

        if (Input.GetKeyDown(KeyCode.Space)) // ура деньги, бесконечность не предел
        {
            AddMoney(10);
        }
    }
}

public enum States
{
    idle,
    move,
    interact
}