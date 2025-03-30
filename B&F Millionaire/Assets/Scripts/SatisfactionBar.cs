using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatisfactionBar : MonoBehaviour
{
    public QueueManager customer;
    public GameObject satisfactionBar;
    public float SatisAmount = 100f;
    public float secondsToAngry = 180f;
    public bool isDecreasing = false;

    void Start()
    {
        satisfactionBar = transform.GetChild(0).gameObject;
        satisfactionBar.transform.localScale = new Vector3(0.5f, 0.07f, 0);
        satisfactionBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SatisAmount > 0 && isDecreasing)
        {
            SatisAmount -= 100 / secondsToAngry * Time.deltaTime * 10;
            satisfactionBar.transform.localScale = new Vector3(SatisAmount/200, 0.07f, 0);
        }
        if(SatisAmount <= 0)
        {
            customer.HandleHeroInteraction();
            Debug.Log("Гость устал ждать и уходит от вас!") 
        }
    }

    public void StartDecreasing()
    {
        isDecreasing = true;
        satisfactionBar.SetActive(true);
    }

    public void StopDecreasing()
    {
        isDecreasing = false;
        satisfactionBar.SetActive(false);
    }
    public void NewBar()
    {
        SatisAmount = 100f;
    }
}
