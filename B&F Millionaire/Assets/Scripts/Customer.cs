using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokypatelDviz : MonoBehaviour
{
    [SerializeField] private List<Transform> TargetPoint = new List<Transform>(); // Список точек 
    public float moveSpeed = 5f; // Скорость движения 
    public float stopDistance = 0.1f; // Расстояние для остановки 

    private Rigidbody2D rb;
    private Vector2 movement;
    private int currentTargetIndex = 0; // Индекс текущей точки 

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

        // Вычисляем направление к текущей точке 
        Vector2 direction = TargetPoint[currentTargetIndex].position - transform.position;

        // Если бот близко к текущей точке, переходим к следующей 
        if (direction.magnitude <= stopDistance)
        {
            NextTarget();
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