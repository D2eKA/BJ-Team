using UnityEngine;
using TMPro;

public class Hero : MonoBehaviour
{
    public int balance = 0;
    public TextMeshProUGUI moneyText;

    [SerializeField] private float speed = 1f;
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
        // Ïîëó÷àåì ââîä è íîðìàëèçóåì âåêòîð íàïðàâëåíèÿ
        Vector2 direction = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Ïðèìåíÿåì äâèæåíèå
        rb.velocity = direction * speed;

        // Óïðàâëåíèå àíèìàöèåé è ïîâîðîòîì
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

        if (Input.GetKeyDown(KeyCode.Space)) // óðà äåíüãè, áåñêîíå÷íîñòü íå ïðåäåë
        {
            AddMoney(10);
        }
    }
    private void HorizontalMove()
    {
        State = States.move;
        Vector2 dir = transform.right * Input.GetAxis("Horizontal");
        Vector2 trans_pos = (Vector2)transform.position;
        transform.position = Vector2.MoveTowards(transform.position, trans_pos + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void VerticalMove()
    {
        State = States.move;
        Vector2 dir = transform.up * Input.GetAxis("Vertical");
        Vector2 trans_pos = (Vector2)transform.position;
        transform.position = Vector3.MoveTowards(transform.position, trans_pos + dir, speed * Time.deltaTime);
    }
}

public enum States
{
    idle,
    move,
    interact
}