using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using Unity.Cinemachine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private LSLStreamDebugger LSLStreamDebugger;
    [SerializeField] private GameObject warningMenu;
    [SerializeField] private GameObject modeMenu;
    [SerializeField] private GameObject transitionManager;

    public void HomeScreen()
    {
        Time.timeScale = 1f;
        transitionManager.SetActive(true);
        TransitionManager.Instance.LoadScene("StartScene");
    }
    public void GameScene()
    {
        Time.timeScale = 1f;
        TransitionManager.Instance.LoadScene("GameScene");
    }
    public void StartManualMode()
    {
        Time.timeScale = 1f;
        TransitionManager.Instance.LoadScene("GameScene");
    }

    public void StartMuseMode()
    {
        if(LSLStreamDebugger.StreamsCount <= 1)
        {
            modeMenu.SetActive(false);
            warningMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            TransitionManager.Instance.LoadScene("GameScene");
        }
    }
}
