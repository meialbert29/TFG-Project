using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PlayerStateMachine playerStateMachine;

    AudioManager audioManager;

    private bool isPaused = false;
    public GameObject pausedUI;

    //public Image pauseButton;
    //public Sprite normal_Sprite;
    //public Sprite hover_Sprite;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        pausedUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
                pausedUI.SetActive(false);
            }
                
            else
            {
                PauseGame();
                pausedUI.SetActive(true);
            }
                
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        playerStateMachine.SetInputEnabled(false); //
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        playerStateMachine.SetInputEnabled(true);
        isPaused = false;
    }

    public void OpenSettingsMenu()
    {

    }
}
