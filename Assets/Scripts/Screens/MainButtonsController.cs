using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainButtonsController : MonoBehaviour
{
    AudioManager audioManager;

    [SerializeField] public Image startButton;
    [SerializeField] private Image settingsButton;
    [SerializeField] private Image stadisticsButton;
    [SerializeField] private Image helpButton;
    [SerializeField] private Image exitButton;

    [SerializeField] private Sprite hoverStart_Sprite;
    [SerializeField] private Sprite normalStart_Sprite;

    [SerializeField] private Sprite hoverSettings_Sprite;
    [SerializeField] private Sprite normalSettings_Sprite;

    [SerializeField] private Sprite hoverHistory_Sprite;
    [SerializeField] private Sprite normalHistory_Sprite;

    [SerializeField] private Sprite helpNormal_Sprite;
    [SerializeField] private Sprite helpHover_Sprite;
    [SerializeField] private Sprite exitNormal_Sprite;
    [SerializeField] private Sprite exitHover_Sprite;
    
    [SerializeField] private TMP_Text startText;
    [SerializeField] private TMP_Text settingsText;
    [SerializeField] private TMP_Text stadisticsText;
    [SerializeField] private TMP_Text helpText;
    [SerializeField] private TMP_Text exitText;

    private Color normalColor = ColorsPalette.ButtonsColors.normalColor;
    private Color hoverColor = ColorsPalette.ButtonsColors.hoverColor;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonPressed);
    }

    private void OnEnable()
    {
        if (startButton != null)
        {
            startButton.sprite = normalStart_Sprite;
            startText.color = normalColor;
        }

        if (settingsButton != null)
        {
            settingsButton.sprite = normalStart_Sprite;
            settingsText.color = normalColor;
        }

        if (stadisticsButton != null)
        {
            stadisticsButton.sprite = normalStart_Sprite;
            stadisticsText.color = normalColor;
        }

        if(helpButton != null)
        {
            helpButton.sprite = helpNormal_Sprite;
            helpText.color = normalColor;
        }
        if(exitButton != null)
        {
            exitButton.sprite = normalStart_Sprite;
            exitText.color = normalColor;
        }
    }

    public void OnStartButtonEnter()
    {
        if (startButton != null)
        {
            startButton.sprite = hoverStart_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }
            
        if (startText != null)
            startText.color = hoverColor;
    }
    public void OnStartButtonExit()
    {
        if (startButton != null)
            startButton.sprite = normalStart_Sprite;
        if (startButton != null)
            startText.color = normalColor;
    }

    public void OnSettingstButtonEnter()
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
            settingsButton.sprite = normalSettings_Sprite;
        if (settingsText != null)
            settingsText.color = normalColor;
    }

    public void OnStadisticsButtonEnter()
    {
        if (stadisticsButton != null)
        {
            stadisticsButton.sprite = hoverHistory_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }
            
        if (stadisticsText != null)
            stadisticsText.color = hoverColor;
    }
    public void OnStadisticsButtonExit()
    {
        if (stadisticsButton != null)
            stadisticsButton.sprite = normalHistory_Sprite;
        if (stadisticsText != null)
            stadisticsText.color = normalColor;
    }

    public void OnHelpButtonEnter()
    {
        if(helpButton != null)
        {
            helpButton.sprite = helpHover_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }
        if (helpText != null)
            helpText.color = hoverColor;
    }

    public void OnHelpButtonExit()
    {
        if(helpButton != null)
            helpButton.sprite = helpNormal_Sprite;
        if(helpText != null)
            helpText.color = normalColor;
    }

    public void OnClickExitButton()
    {
        #if UNITY_EDITOR
              UnityEditor.EditorApplication.isPlaying = false;
        #else
              Application.Quit();
        #endif
    }

    public void OnExitButtonEnter()
    {
        if (exitButton != null)
        {
            exitButton.sprite = exitHover_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }
        if (exitText != null)
            exitText.color = hoverColor;
    }

    public void OnExitButtonExit()
    {
        if (exitButton != null)
            exitButton.sprite = exitNormal_Sprite;
        if (exitText != null)
            exitText.color = normalColor;
    }
}
