using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pause_menu;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeManager.Instance.ToggleUpgradePanel();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShopManager.Instance.ToggleShop();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause_menu.activeSelf)
                Continue();
            else
                Pause();
        }
    }
    public void Continue()
    {
        pause_menu.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void Pause()
    {
        pause_menu.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void Exit()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main Menu");
    }

}
