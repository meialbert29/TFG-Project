using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }
    public void SettingsMenu()
    {
        SceneManager.LoadSceneAsync("SettingsMenuScene");
    }
    public void StadisticsMenu()
    {
        SceneManager.LoadSceneAsync("TrackerScoreScene");
    }
}
