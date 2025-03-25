using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatisfactionBar : MonoBehaviour
{
    public Image satisfactionBar;
    public float SatisAmount = 100f;
    public float secondsToAngry = 60f;
    void Start()
    {
        satisfactionBar.fillAmount = SatisAmount / 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (SatisAmount > 0) 
        {
            SatisAmount -= 100 / secondsToAngry * Time.deltaTime;
            satisfactionBar.fillAmount = SatisAmount / 100;
        }
        
    }
}
