using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameResultUI : MonoBehaviour
{
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    void Start()
    {
        resultPanel.SetActive(false);
    }

    public void ShowWinner(string winnerName)
    {
        resultPanel.SetActive(true);
        resultText.text = winnerName + " Win!";

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
