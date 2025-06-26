using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.LSL4Unity.Scripts.AbstractInlets;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private LSLStreamDebugger LSLStreamDebugger;
    [SerializeField] private GameObject warningMenu;
    [SerializeField] private GameObject modeMenu;

    public void HomeScreen()
    {
        Time.timeScale = 1f;
        TransitionManager.Instance.LoadScene("StartScene");
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
