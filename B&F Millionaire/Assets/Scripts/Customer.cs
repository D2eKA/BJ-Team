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

    // Новые поля для запроса
    [SerializeField] private SpriteRenderer requestIconRenderer; // Компонент для отображения иконки
    [SerializeField] private GameObject requestBubble; // Пузырь запроса (родительский объект)
    private Item.ItemType requestedItem = Item.ItemType.None;

    // Свойство для доступа к запрашиваемому товару
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

    // Обновление иконки запроса
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

    // Показать запрос
    public void ShowRequest()
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(true);
        }
    }

    // Скрыть запрос
    public void HideRequest()
    {
        if (requestBubble != null)
        {
            requestBubble.SetActive(false);
        }
    }

    // Существующие методы
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
