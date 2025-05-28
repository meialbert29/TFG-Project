using UnityEngine;
using UnityEngine.SceneManagement;

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
