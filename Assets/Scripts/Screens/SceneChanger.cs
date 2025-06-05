using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public void GameScreen()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void HomeScreen()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
    }
}
