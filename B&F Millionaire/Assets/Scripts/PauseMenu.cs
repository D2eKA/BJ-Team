using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pause_menu;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Закрываем другие меню перед открытием меню улучшений
            if (pause_menu.activeSelf)
                Continue();
                
            if (ShopManager.Instance != null)
                ShopManager.Instance.CloseShop();
                
            UpgradeManager.Instance.ToggleUpgradePanel();
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Закрываем другие меню перед открытием магазина
            if (pause_menu.activeSelf)
                Continue();
                
            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.CloseUpgradePanel();
                
            ShopManager.Instance.ToggleShop();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause_menu.activeSelf)
            {
                Continue();
            }
            else
            {
                // Закрываем другие меню перед открытием меню паузы
                if (UpgradeManager.Instance != null)
                    UpgradeManager.Instance.CloseUpgradePanel();
                    
                if (ShopManager.Instance != null)
                    ShopManager.Instance.CloseShop();
                    
                Pause();
            }
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
