//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PokypatelDviz : MonoBehaviour
//{
//    [SerializeField] private List<Transform> TargetPoint = new List<Transform>(); // ������ �����
//    [SerializeField] private Transform table; // ������ table
//    [SerializeField] private Transform Hero; // ������� ��������
//    [SerializeField] private float interactionDistance = 2f; // ���������� ��������������
//    [SerializeField] private Queue<Transform> QueuePosition = new Queue<Transform>(); // ������ �����
//    public float moveSpeed = 5f; // �������� ��������
//    public float stopDistance = 0.1f; // ���������� ��� ���������

//    private Rigidbody2D rb; 
//    private Vector2 movement;
//    private int currentTargetIndex = 0; // ������ ������� �����
//    private bool waitingAtTable = false; // ���� �������� � table

//    void Start()
//    {
//        // �������� ��������� Rigidbody2D
//        rb = GetComponent<Rigidbody2D>();

//        // ��������, ��� ������ �� ����
//        if (TargetPoint.Count > 0)
//        {
//            // ������������� ������ ����
//            SetTargetPoint(TargetPoint[currentTargetIndex]);
//        }
//    }

//    void Update()
//    {
//        // ���� ������ ����� ����, ������ �� ������
//        if (TargetPoint.Count == 0) return;

//        // ���������, ������� �� � table
//        if (waitingAtTable)
//        {
//            // ��������� ���������� �� �������� ���������
//            if (Vector2.Distance(transform.position, Hero.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
//            {
//                waitingAtTable = false; // ��������� ��������� ������
//                NextTarget(); // ������� � ��������� �����
//            }
//            return; // ���� ����, ������ ������ �� ������
//        }

//        // ��������� ����������� � ������� �����
//        Vector2 direction = TargetPoint[currentTargetIndex].position - transform.position;

//        // ���� ��� ������ � ������� �����, ��������� � ���������
//        if (direction.magnitude <= stopDistance)
//        {
//            // ���������, �������� �� ������� table
//            if (TargetPoint[currentTargetIndex] == table)
//            {
//                waitingAtTable = true; // ��������������� � table
//                movement = Vector2.zero;
//                rb.velocity = Vector2.zero;
//            }
//            else
//            {
//                NextTarget();
//            }
//        }
//        else
//        {
//            // ����������� ����������� � ��������� ��������
//            movement = direction.normalized * moveSpeed;
//        }
//    }

//    void FixedUpdate()
//    {
//        // ����������� ����
//        if (movement != Vector2.zero)
//        {
//            rb.velocity = movement;
//        }
//        else
//        {
//            rb.velocity = Vector2.zero;
//        }
//    }

//    // ������������� ����� ����� ����
//    private void SetTargetPoint(Transform newTarget)
//    {
//        if (newTarget != null)
//        {
//            movement = Vector2.zero;
//        }
//    }

//    // ������� � ��������� ����� � ������
//    private void NextTarget()
//    {
//        currentTargetIndex++;

//        // ���� �������� ����� ������, ������� ������
//        if (currentTargetIndex >= TargetPoint.Count)
//        {
//            Destroy(gameObject); // ���������� ������
//            return;
//        }
//        // ������������� ����� ����
//        SetTargetPoint(TargetPoint[currentTargetIndex]);
//    } 

//}
using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 2f;
    private bool isMoving = false;
    private Action onReachedTarget;
    private bool readyForInteraction = false;

    private void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    public void MoveTo(Vector3 position, Action onComplete = null)
    {
        targetPosition = position;
        onReachedTarget = onComplete;
        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            onReachedTarget?.Invoke();
        }
    }

    public bool IsReadyForInteraction()
    {
        return readyForInteraction;
    }

    public void SetReadyForInteraction(bool ready)
    {
        readyForInteraction = ready;
    }
}