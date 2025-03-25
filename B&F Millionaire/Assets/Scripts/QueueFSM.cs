﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerMovement : MonoBehaviour
{
    [SerializeField] private Transform table; // Ссылка на стол, к которому направляются покупатели
    [SerializeField] private Transform hero; // Главный персонаж (герой), с которым покупатель взаимодействует
    [SerializeField] private List<Transform> waypoints = new List<Transform>(); // Маршрут, по которому покупатели идут перед посещением очереди
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // Позиции для формирования очереди
    [SerializeField] private float interactionDistance = 2f; // Расстояние для взаимодействия с героем
    [SerializeField] private float moveSpeed = 5f; // Скорость движения покупателя
    [SerializeField] private float stopDistance = 0.1f; // Минимальное расстояние для остановки перед целью

    private Rigidbody2D rb; // Ссылка на Rigidbody2D компонента для управления движением
    private Vector2 movement; // Направление движения
    private Transform currentTarget; // Текущая цель, к которой движется покупатель

    private static bool isTableOccupied = false; // Флаг, указывающий, занят ли стол
    private static Queue<BuyerMovement> waitingQueue = new Queue<BuyerMovement>(); // Очередь покупателей

    private int waypointIndex = 0; // Индекс текущей точки в маршруте
    private bool reachedWaypoints = false; // Флаг, указывающий, завершил ли покупатель маршрут

    // Состояния покупателя
    private enum BuyerState
    {
        MovingToWaypoints,   // Движение по маршруту (waypoints)
        MovingToTable,       // Движение к столу
        WaitingInQueue,      // Ожидание в очереди
        InteractingWithTable, // Взаимодействие с героем за столом
        Leaving              // Уход с карты
    }

    private BuyerState currentState; // Текущее состояние покупателя

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = BuyerState.MovingToWaypoints; // Начальное состояние - движение по маршруту
        SetTarget(waypoints[0]); // Устанавливаем первую точку маршрута как цель
    }

    void Update()
    {
        // Вызываем поведение, соответствующее текущему состоянию
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
        // Устанавливаем скорость Rigidbody2D, если движение активно
        rb.velocity = movement != Vector2.zero ? movement : Vector2.zero;
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target; // Устанавливаем текущую цель
        movement = Vector2.zero; // Сбрасываем движение
        Debug.Log($"Новая цель: {target.position}"); // Для отладки
    }

    private void MoveToWaypoints()
    {
        if (waypoints.Count == 0 || waypointIndex >= waypoints.Count)
        {
            // Если маршрут завершен, переходим к уходу
            currentState = BuyerState.Leaving;
            return;
        }

        // Устанавливаем текущую точку маршрута как цель
        SetTarget(waypoints[waypointIndex]);
        MoveToTarget();

        // Проверяем, достиг ли покупатель текущей точки маршрута
        if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) <= stopDistance)
        {
            waypointIndex++; // Переходим к следующей точке маршрута
        }
    }

    private void MoveToTarget()
    {
        if (currentTarget == null) return;

        Vector2 direction = (currentTarget.position - transform.position).normalized; // Направление движения
        float distance = Vector2.Distance(transform.position, currentTarget.position); // Расстояние до цели

        if (distance <= stopDistance)
        {
            // Если достигли цели
            movement = Vector2.zero;

            if (currentTarget == table) // Если цель - стол
            {
                if (isTableOccupied)
                {
                    JoinQueue(); // Если стол занят, встаем в очередь
                }
                else
                {
                    isTableOccupied = true; // Занимаем стол
                    currentState = BuyerState.InteractingWithTable; // Переходим в состояние взаимодействия
                }
            }
        }
        else
        {
            // Если не достигли цели, продолжаем движение
            movement = direction * moveSpeed;
        }
    }

    private void JoinQueue()
    {
        // Если нет доступных мест в очереди, покупатель уходит
        if (queuePositions.Count == 0)
        {
            Debug.LogError("Нет мест для очереди!");
            currentState = BuyerState.Leaving;
            return;
        }

        // Ищем свободную позицию в очереди
        foreach (Transform queuePoint in queuePositions)
        {
            if (!IsPositionOccupied(queuePoint))
            {
                waitingQueue.Enqueue(this); // Добавляем покупателя в очередь
                SetTarget(queuePoint); // Устанавливаем позицию как цель
                currentState = BuyerState.WaitingInQueue; // Меняем состояние на ожидание
                Debug.Log($"Покупатель занял очередь на позиции: {queuePoint.position}");
                return;
            }
        }

        // Если очередь заполнена, покупатель уходит
        Debug.LogWarning("Очередь заполнена! Покупатель уходит.");
        currentState = BuyerState.Leaving;
    }

    private void WaitForTable()
    {
        // Покупатель ждет, пока стол не освободится
        if (!isTableOccupied && waitingQueue.Peek() == this)
        {
            waitingQueue.Dequeue(); // Убираем покупателя из очереди
            currentState = BuyerState.MovingToTable; // Направляем его к столу
            SetTarget(table);
        }
    }

    private void WaitForHeroInteraction()
    {
        // Проверяем, находится ли покупатель в зоне взаимодействия
        if (Vector2.Distance(transform.position, hero.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Покупатель завершил взаимодействие со столом.");
            isTableOccupied = false; // Освобождаем стол
            currentState = reachedWaypoints ? BuyerState.Leaving : BuyerState.MovingToWaypoints;

            // Уведомляем следующего покупателя в очереди
            if (waitingQueue.Count > 0)
            {
                waitingQueue.Peek().NotifyTableAvailable();
            }
        }
    }

    private void LeaveScene()
    {
        Debug.Log("Покупатель уходит.");
        Destroy(gameObject); // Уничтожаем объект покупателя
    }

    private bool IsPositionOccupied(Transform position)
    {
        // Проверяем, занята ли позиция в очереди другим покупателем
        foreach (GameObject buyer in GameObject.FindGameObjectsWithTag("Buyer"))
        {
            if (Vector2.Distance(buyer.transform.position, position.position) <= stopDistance)
            {
                return true; // Позиция занята
            }
        }
        return false; // Позиция свободна
    }

    public void NotifyTableAvailable()
    {
        // Уведомляем покупателя, что он может направляться к столу
        if (currentState == BuyerState.WaitingInQueue)
        {
            currentState = BuyerState.MovingToTable;
            SetTarget(table);
        }
    }
}
