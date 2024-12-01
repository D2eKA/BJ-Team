using TMPro;
using UnityEngine;

public class HeroInteraction : MonoBehaviour
{
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private Transform hero;
    [SerializeField] private float interactionDistance = 2f;
    public GameObject invWindow;
    public TextMeshProUGUI moneyText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&& Vector2.Distance(transform.position, hero.position) <= interactionDistance)
        {
            queueManager.HandleHeroInteraction();
            ClearInventory();
            
        }
        else {return; }
    }
    private void ClearInventory()
    {
        Hero lol = hero.gameObject.GetComponent<Hero>();
        Inventory inv = hero.gameObject.GetComponent<Inventory>();
        lol.balance += inv.ValInvetory;
        inv.ValInvetory = 0;
        inv.Items.Clear(); 

        int count = invWindow.transform.childCount;
        while (count > 0)
        {
            Destroy(invWindow.transform.GetChild(0).gameObject);
            count--;
        }
        moneyText.text = lol.balance.ToString();
    }
}