using UnityEngine;
using TMPro;

public class Hero : MonoBehaviour
{
    public int balance = 0;
    public TextMeshProUGUI moneyText;

    [SerializeField] public float speed = 1f;
    
    [Header("Аудио")]
    [SerializeField] private AudioSource footstepsAudioSource; // Ссылка на аудио-источник
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool isMoving = false;

    private void Start()
    {
        UpdateMoneyDisplay();
        
        // Проверяем наличие аудио-источника
        if (footstepsAudioSource == null)
        {
            footstepsAudioSource = GetComponent<AudioSource>();
            if (footstepsAudioSource == null)
            {
                footstepsAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Настраиваем аудио-источник
        footstepsAudioSource.loop = true;
        footstepsAudioSource.playOnAwake = false;
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
        set 
        {
            anim.SetInteger("state", (int)value);
            
            // Управляем звуком шагов
            if (value == States.move)
            {
                PlayFootsteps();
            }
            else
            {
                StopFootsteps();
            }
        }
    }
    
    // Включаем звук шагов
    private void PlayFootsteps()
    {
        if (!isMoving && footstepsAudioSource != null && footstepsAudioSource.clip != null)
        {
            footstepsAudioSource.Play();
            isMoving = true;
        }
    }
    
    // Выключаем звук шагов
    private void StopFootsteps()
    {
        if (isMoving && footstepsAudioSource != null)
        {
            footstepsAudioSource.Stop();
            isMoving = false;
        }
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
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space)) // ура деньги, бесконечность не предел
        {
            AddMoney(10);
        }
#endif
    }
}

public enum States
{
    idle,
    move,
    interact
}
