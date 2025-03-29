using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatisfactionBar : MonoBehaviour
{
    public Image satisfactionBar;
    public float SatisAmount = 100f;
    public float secondsToAngry = 60f;
    private bool isDecreasing = false;

    void Start()
    {
        satisfactionBar.fillAmount = SatisAmount / 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDecreasing && SatisAmount > 0)
        {
            SatisAmount -= 100 / secondsToAngry * Time.deltaTime;
            satisfactionBar.fillAmount = SatisAmount / 100;
        }

    }

    public void StartDecreasing()
    {
        isDecreasing = true;
    }

    public void StopDecreasing()
    {
        isDecreasing = false;
    }
    public void NewBar()
    {
        SatisAmount = 100f;
        satisfactionBar.fillAmount = SatisAmount / 100;
    }
}
