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
    public void StartManualMode()
    {
        Time.timeScale = 1f;
        //PlayerPrefs.SetInt("GameMode", 0); // 0 = manual
        SceneManager.LoadScene("GameScene");
    }

    public void StartMuseMode()
    {
        Time.timeScale = 1f;
        //PlayerPrefs.SetInt("GameMode", 1); // 1 = muse
        SceneManager.LoadScene("GameScene");
    }
}
