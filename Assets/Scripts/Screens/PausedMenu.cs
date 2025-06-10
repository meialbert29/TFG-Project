using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PlayerStateMachine playerStateMachine;

    AudioManager audioManager;

    private bool isPaused = false;
    public GameObject pausedUI;
    public GameObject settingsMenu;

    //public Image pauseButton;
    //public Sprite normal_Sprite;
    //public Sprite hover_Sprite;

    public Image resumeButton;
    public TMP_Text resumeText;

    public Image settingsButton;
    public TMP_Text settingsText;

    public Image quitButton;
    public TMP_Text quitText;

    public Sprite normalResume_Sprite;
    public Sprite hoverResume_Sprite;
    public Sprite normalSettings_Sprite;
    public Sprite hoverSettings_Sprite;
    public Sprite normalQuit_Sprite;
    public Sprite hoverQuit_Sprite;

    private Color normalColor = ColorsPalette.ButtonsColors.normalColor;
    private Color hoverColor = ColorsPalette.ButtonsColors.hoverColor;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnEnable()
    {
        if(resumeButton != null && settingsButton != null && quitButton != null)
        {
            resumeButton.sprite = normalResume_Sprite;
            settingsButton.sprite = normalSettings_Sprite;
            quitButton.sprite = normalQuit_Sprite;

            resumeText.color = normalColor;
            settingsText.color = normalColor;
            quitText.color = normalColor;
        }
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
                PauseGame(); 
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        playerStateMachine.SetInputEnabled(false); //
        isPaused = true;
        pausedUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        playerStateMachine.SetInputEnabled(true);
        isPaused = false;
    }

    public void OnResumeButtonEnter()
    {
        if (resumeButton != null)
        {
            resumeButton.sprite = hoverResume_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (resumeText != null)
            resumeText.color = hoverColor;
    }
    public void OnResumeButtonExit()
    {
        if (resumeButton != null)
        {
            resumeButton.sprite = normalResume_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (resumeText != null)
            resumeText.color = normalColor;
    }

    public void OnSettingsButtonEnter()
    {
        if (settingsButton != null)
        {
            settingsButton.sprite = hoverSettings_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (settingsText != null)
            settingsText.color = hoverColor;
    }
    public void OnSettingsButtonExit()
    {
        if (settingsButton != null)
        {
            settingsButton.sprite = normalSettings_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (resumeText != null)
            resumeText.color = normalColor;
    }

    public void OnQuitButtonEnter()
    {
        if (quitButton != null)
        {
            quitButton.sprite = hoverQuit_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (quitText != null)
            quitText.color = hoverColor;
    }
    public void OnQuitButtonExit()
    {
        if (quitButton != null)
        {
            quitButton.sprite = normalQuit_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (quitText != null)
            quitText.color = normalColor;
    }

    public void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonPressed);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
    }
}
