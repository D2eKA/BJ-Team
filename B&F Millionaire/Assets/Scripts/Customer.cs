using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokypatelDviz : MonoBehaviour
{
    [SerializeField] private List<Transform> TargetPoint = new List<Transform>(); // Список точек
    [SerializeField] private Transform table; // Объект table
    [SerializeField] private Transform Hero; // Главный персонаж
    [SerializeField] private float interactionDistance = 2f; // Расстояние взаимодействия
    [SerializeField] private Queue<Transform> QueuePosition = new Queue<Transform>(); // Список точек
    public float moveSpeed = 5f; // Скорость движения
    public float stopDistance = 0.1f; // Расстояние для остановки

    private Rigidbody2D rb; 
    private Vector2 movement;
    private int currentTargetIndex = 0; // Индекс текущей точки
    private bool waitingAtTable = false; // Флаг ожидания у table

    void Start()
    {
        // Получаем компонент Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Убедимся, что список не пуст
        if (TargetPoint.Count > 0)
        {
            // Устанавливаем первую цель
            SetTargetPoint(TargetPoint[currentTargetIndex]);
        }
    }

    void Update()
    {
        // Если список точек пуст, ничего не делаем
        if (TargetPoint.Count == 0) return;

        // Проверяем, ожидаем ли у table
        if (waitingAtTable)
        {
            // Проверяем расстояние до главного персонажа
            if (Vector2.Distance(transform.position, Hero.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
            {
                waitingAtTable = false; // Разрешаем двигаться дальше
                NextTarget(); // Переход к следующей точке
            }
            return; // Пока ждем, ничего больше не делаем
        }

        // Вычисляем направление к текущей точке
        Vector2 direction = TargetPoint[currentTargetIndex].position - transform.position;

        // Если бот близко к текущей точке, переходим к следующей
        if (direction.magnitude <= stopDistance)
        {
            // Проверяем, достигли ли объекта table
            if (TargetPoint[currentTargetIndex] == table)
            {
                waitingAtTable = true; // Останавливаемся у table
                movement = Vector2.zero;
                rb.velocity = Vector2.zero;
            }
            else
            {
                NextTarget();
            }
        }
        else
        {
            // Нормализуем направление и вычисляем движение
            movement = direction.normalized * moveSpeed;
        }
    }

    void FixedUpdate()
    {
        // Перемещение бота
        if (movement != Vector2.zero)
        {
            rb.velocity = movement;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Устанавливаем новую точку цели
    private void SetTargetPoint(Transform newTarget)
    {
        if (newTarget != null)
        {
            movement = Vector2.zero;
        }
    }

    // Переход к следующей точке в списке
    private void NextTarget()
    {
        currentTargetIndex++;

        // Если достигли конца списка, удаляем объект
        if (currentTargetIndex >= TargetPoint.Count)
        {
            Destroy(gameObject); // Уничтожаем объект
            return;
        }
        // Устанавливаем новую цель
        SetTargetPoint(TargetPoint[currentTargetIndex]);
    } 

}
