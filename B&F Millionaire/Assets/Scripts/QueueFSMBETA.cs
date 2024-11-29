//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PokypatelDviz : MonoBehaviour
//{
//    [SerializeField] private Transform table; // Стол, к которому идут покупатели
//    [SerializeField] private Transform Hero; // Главный персонаж
//    [SerializeField] private List<Transform> QueuePositions = new List<Transform>(); // Точки для очереди
//    [SerializeField] private float interactionDistance = 2f; // Дистанция взаимодействия
//    [SerializeField] private float moveSpeed = 5f; // Скорость движения
//    [SerializeField] private float stopDistance = 0.1f; // Расстояние для остановки

//    private Rigidbody2D rb;
//    private Vector2 movement;
//    private Transform currentTarget; // Текущая цель

//    private static bool isTableOccupied = false; // Флаг, показывающий занятость стола
//    private static Queue<Transform> WaitingQueue = new Queue<Transform>(); // Очередь ожидания

//    private enum BuyerState
//    {
//        MovingToTable,
//        WaitingInQueue,
//        InteractingWithTable,
//        Leaving
//    }

//    private BuyerState currentState;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        currentState = BuyerState.MovingToTable;
//        SetTarget(table); // Изначально двигаемся к столу
//    }

//    void Update()
//    {
//        switch (currentState)
//        {
//            case BuyerState.MovingToTable:
//                MoveToTarget();
//                break;

//            case BuyerState.WaitingInQueue:
//                WaitForTable();
//                break;

//            case BuyerState.InteractingWithTable:
//                InteractWithTable();
//                break;

//            case BuyerState.Leaving:
//                LeaveScene();
//                break;
//        }
//    }

//    void FixedUpdate()
//    {
//        if (movement != Vector2.zero)
//        {
//            rb.velocity = movement;
//        }
//        else
//        {
//            rb.velocity = Vector2.zero;
//        }
//    }

//    private void SetTarget(Transform target)
//    {
//        currentTarget = target;
//        movement = Vector2.zero;
//    }

//    private void MoveToTarget()
//    {
//        if (currentTarget == null) return;

//        Vector2 direction = (currentTarget.position - transform.position).normalized;
//        float distance = Vector2.Distance(transform.position, currentTarget.position);

//        if (distance <= stopDistance)
//        {
//            if (currentTarget == table)
//            {
//                if (isTableOccupied)
//                {
//                    JoinQueue();
//                }
//                else
//                {
//                    isTableOccupied = true;
//                    currentState = BuyerState.InteractingWithTable;
//                }
//            }
//            else
//            {
//                currentState = BuyerState.MovingToTable;
//                SetTarget(table);
//            }
//        }
//        else
//        {
//            movement = direction * moveSpeed;
//        }
//    }

//    private void JoinQueue()
//    {
//        if (QueuePositions.Count == 0)
//        {
//            Debug.LogError("Нет точек для очереди!");
//            return;
//        }

//        foreach (Transform queuePoint in QueuePositions)
//        {
//            if (!IsPositionOccupied(queuePoint))
//            {
//                WaitingQueue.Enqueue(transform);
//                SetTarget(queuePoint);
//                currentState = BuyerState.WaitingInQueue;
//                return;
//            }
//        }

//        Debug.LogWarning("Очередь заполнена! Покупатель уходит.");
//        currentState = BuyerState.Leaving; // Если нет места, уходим
//    }

//    private void WaitForTable()
//    {
//        if (!isTableOccupied && WaitingQueue.Peek() == transform)
//        {
//            WaitingQueue.Dequeue();
//            currentState = BuyerState.MovingToTable;
//            SetTarget(table);
//        }
//    }

//    private void InteractWithTable()
//    {
//        if (Vector2.Distance(transform.position, Hero.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
//        {
//            Debug.Log("Покупатель завершил взаимодействие со столом.");
//            isTableOccupied = false;
//            currentState = BuyerState.Leaving;

//            if (WaitingQueue.Count > 0)
//            {
//                var nextBuyer = WaitingQueue.Peek();
//                nextBuyer.GetComponent<PokypatelDviz>().NotifyTableAvailable();
//            }
//        }
//    }

//    private void LeaveScene()
//    {
//        Debug.Log("Покупатель уходит.");
//        Destroy(gameObject);
//    }

//    private bool IsPositionOccupied(Transform position)
//    {
//        foreach (GameObject buyer in GameObject.FindGameObjectsWithTag("Buyer"))
//        {
//            if (Vector2.Distance(buyer.transform.position, position.position) <= stopDistance)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    public void NotifyTableAvailable()
//    {
//        if (currentState == BuyerState.WaitingInQueue)
//        {
//            currentState = BuyerState.MovingToTable;
//            SetTarget(table);
//        }
//    }
//}
