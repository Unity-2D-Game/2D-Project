using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public Image hpBar;
    public GameObject gameOverUI;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPBar();
        gameOverUI.SetActive(false);
    }
   
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        UpdateHPBar();
        if (currentHP <= 0)
        {
            GameOver();
        }
    }
    
    void UpdateHPBar()
    {
        hpBar.fillAmount = (float)currentHP / maxHP;
    }
   
    void GameOver()
    {
        Debug.Log("게임 종료");
        gameOverUI.SetActive(true);
        gameObject.SetActive(false);
        Time.timeScale = 0f;
    }
   
    public void RestartGame()
    {   
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
