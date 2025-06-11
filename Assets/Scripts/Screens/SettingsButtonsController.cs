using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
public class SettingsButtonsController : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] SaveSystem saveSystem;

    // images variables
    public Image saveButton;
    public TMP_Text saveText;

    public Image clearHistoryButton;
    public TMP_Text clearHistoryText;

    // sprites variables
    public Sprite normalSaveButton_Sprite;
    public Sprite hoverSaveButton_Sprite;

    public Sprite normalClearButton_Sprite;
    public Sprite hoverClearButton_Sprite;

    private Color normalColor = ColorsPalette.ButtonsColors.normalColor;
    private Color hoverColor = ColorsPalette.ButtonsColors.hoverColor;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        if(audioManager == null)
            Debug.Log("Audio manager not found");
        if (saveSystem == null)
            Debug.Log("Save system not found");
        
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (clearHistoryButton != null)
                clearHistoryButton.gameObject.SetActive(false);

            if (clearHistoryText != null)
                clearHistoryText.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (saveButton != null)
        {
            saveButton.sprite = normalSaveButton_Sprite;
            saveText.color = normalColor;

            //clearHistoryButton.sprite = normalSaveButton_Sprite;
            //clearHistoryText.color = normalColor;
        }
    }

    public void OnSaveButtonEnter()
    {
        if (saveButton != null)
        {
            saveButton.sprite = hoverSaveButton_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }
            
        if (saveText != null)
            saveText.color = hoverColor;
    }
    public void OnSaveButtonExit()
    {
        if (saveButton != null)
            saveButton.sprite = normalSaveButton_Sprite;
        if (saveText != null)
            saveText.color = normalColor;
    }

    public void OnClearHistoryButtonEnter()
    {
        if (clearHistoryButton != null)
        {
            clearHistoryButton.sprite = hoverClearButton_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (clearHistoryText != null)
            clearHistoryText.color = hoverColor;
    }

    public void OnClearHistoryButtonExit()
    {
        if (clearHistoryButton != null)
            clearHistoryButton.sprite = normalClearButton_Sprite;
        if (clearHistoryText != null)
            clearHistoryText.color = normalColor;
    }

    public void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonPressed);
    }

    public void OnClearHistoryButtonClick()
    {
        saveSystem.ClearAllScoreEntries();
    }
}
