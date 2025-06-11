using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public void GameScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    public void HomeScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
    }
}
