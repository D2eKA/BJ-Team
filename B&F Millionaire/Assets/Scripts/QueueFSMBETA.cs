using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerMovement : MonoBehaviour
{
    [SerializeField] private Transform table; // Стол, к которому идут покупатели
    [SerializeField] private Transform hero; // Главный персонаж
    [SerializeField] private List<Transform> waypoints = new List<Transform>(); // Точки для посещения перед и после стола
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // Позиции для очереди
    [SerializeField] private float interactionDistance = 2f; // Дистанция взаимодействия
    [SerializeField] private float moveSpeed = 5f; // Скорость движения
    [SerializeField] private float stopDistance = 0.1f; // Расстояние для остановки

    private Rigidbody2D rb;
    private Vector2 movement;
    private Transform currentTarget; // Текущая цель

    private static bool isTableOccupied = false; // Флаг, показывающий занятость стола
    private static Queue<BuyerMovement> waitingQueue = new Queue<BuyerMovement>(); // Очередь покупателей

    private int waypointIndex = 0; // Индекс текущей точки маршрута
    private bool reachedWaypoints = false; // Флаг завершения маршрута

    private enum BuyerState
    {
        MovingToWaypoints,
        MovingToTable,
        WaitingInQueue,
        InteractingWithTable,
        Leaving
    }

    private BuyerState currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = BuyerState.MovingToWaypoints;
        SetTarget(waypoints[0]); // Изначально двигаемся к первой точке
    }

    void Update()
    {
        switch (currentState)
        {
            case BuyerState.MovingToWaypoints:
                MoveToWaypoints();
                break;
            case BuyerState.MovingToTable:
                MoveToTarget();
                break;
            case BuyerState.WaitingInQueue:
                WaitForTable();
                break;
            case BuyerState.InteractingWithTable:
                WaitForHeroInteraction();
                break;
            case BuyerState.Leaving:
                LeaveScene();
                break;
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            rb.velocity = movement;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
        movement = Vector2.zero;
    }

    private void MoveToWaypoints()
{
    if (waypoints.Count == 0 || waypointIndex >= waypoints.Count)
    {
        // Все точки маршрута пройдены
        currentState = BuyerState.Leaving; // Покупатель уходит
        return;
    }

    // Устанавливаем текущую точку маршрута как цель
    SetTarget(waypoints[waypointIndex]);
    MoveToTarget();

    // Проверяем, достиг ли покупатель текущей точки
    if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) <= stopDistance)
    {
        waypointIndex++; // Переходим к следующей точке маршрута
    }
}

    private void MoveToTarget()
    {
        if (currentTarget == null) return;

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance <= stopDistance)
        {
            if (currentTarget == table)
            {
                if (isTableOccupied)
                {
                    JoinQueue();
                }
                else
                {
                    isTableOccupied = true;
                    currentState = BuyerState.InteractingWithTable;
                }
            }
        }
        else
        {
            movement = direction * moveSpeed;
        }
    }

    private void JoinQueue()
    {
        if (queuePositions.Count == 0)
        {
            Debug.LogError("Нет точек для очереди!");
            currentState = BuyerState.Leaving;
            return;
        }

        foreach (Transform queuePoint in queuePositions)
        {
            if (!IsPositionOccupied(queuePoint)) // Проверяем, занята ли точка
            {
                waitingQueue.Enqueue(this); // Добавляем покупателя в очередь
                SetTarget(queuePoint); // Устанавливаем точку очереди как цель
                currentState = BuyerState.WaitingInQueue; // Изменяем состояние
                Debug.Log($"Покупатель занял очередь на позиции: {queuePoint.position}");
                return;
            }
        }

        // Если все точки заняты, покупатель уходит
        Debug.LogWarning("Очередь заполнена! Покупатель уходит.");
        currentState = BuyerState.Leaving;
    }

    private void WaitForTable()
    {
        if (!isTableOccupied && waitingQueue.Peek() == this)
        {
            waitingQueue.Dequeue();
            currentState = BuyerState.MovingToTable;
            SetTarget(table);
        }
    }

    private void WaitForHeroInteraction()
    {
        if (Vector2.Distance(transform.position, hero.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Покупатель завершил взаимодействие со столом.");
            isTableOccupied = false;
            currentState = reachedWaypoints ? BuyerState.Leaving : BuyerState.MovingToWaypoints;

            // Удаляем покупателя из очереди после завершения взаимодействия
            if (waitingQueue.Contains(this))
            {
                waitingQueue.Dequeue(); // Удаляем текущего покупателя из очереди
            }

            if (waitingQueue.Count > 0)
            {
                waitingQueue.Peek().NotifyTableAvailable(); // Уведомляем следующего покупателя
            }
        }
    }

    private void LeaveScene()
    {
        Debug.Log("Покупатель уходит.");
        Destroy(gameObject);
    }

    private bool IsPositionOccupied(Transform position)
    {
        foreach (GameObject buyer in GameObject.FindGameObjectsWithTag("Buyer"))
        {
            if (Vector2.Distance(buyer.transform.position, position.position) <= stopDistance)
            {
                return true; // Точка занята
            }
        }
        return false; // Точка свободна
    }


    public void NotifyTableAvailable()
    {
        if (currentState == BuyerState.WaitingInQueue)
        {
            currentState = BuyerState.MovingToTable;
            SetTarget(table);
        }
    }
}
