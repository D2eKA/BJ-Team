using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 2f;
    private bool isMoving = false;
    private Action onReachedTarget;
    private bool readyForInteraction = false;
    public bool onTable;

    // ����� ���� ��� �������
    [SerializeField] private SpriteRenderer requestIconRenderer; // ��������� ��� ����������� ������
    [SerializeField] private GameObject requestBubble; // ������ ������� (������������ ������)
    private Item.ItemType requestedItem = Item.ItemType.None;

    // �������� ��� ������� � �������������� ������
    public Item.ItemType RequestedItem
    {
        get { return requestedItem; }
        set 
        { 
            requestedItem = value;
            UpdateRequestIcon();
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    // ���������� ������ �������
    private void UpdateRequestIcon()
    {
        if (requestIconRenderer != null && ProductManager.Instance != null && requestedItem != Item.ItemType.None)
        {
            Sprite productSprite = ProductManager.Instance.GetProductSprite(requestedItem);
            if (productSprite != null)
            {
                requestIconRenderer.sprite = productSprite;
            }
        }
    }

    // �������� ������
    public void ShowRequest()
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(true);
        }
    }

    // ������ ������
    public void HideRequest()
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }
    }

    // ������������ ������
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
