using UnityEngine;

public class HeroInteraction : MonoBehaviour
{
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private Transform hero;
    [SerializeField] private float interactionDistance = 2f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&& Vector2.Distance(transform.position, hero.position) <= interactionDistance)
        {
            queueManager.HandleHeroInteraction();
        }
        else {return; }
    }
}