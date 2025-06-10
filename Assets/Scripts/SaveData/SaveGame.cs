using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame : MonoBehaviour
{
    AudioManager audioManager;
    public SaveSystem saveSystem;
    public ScoreSystem scoreSystem;

    public Image saveButton;
    public TMP_Text saveText;
    public Sprite normalSave_Sprite;
    public Sprite hoverSave_Sprite;

    public Image exitButton;
    public TMP_Text exitText;
    public Sprite normalExit_Sprite;
    public Sprite hoverExitSprite;

    public Image staticsButton;
    public Image exitGameButton;
    public Image newGameButton;

    public Sprite normal_StaticsButton;
    public Sprite hover_StaticsButton;
    public Sprite normal_ExitGameButton;
    public Sprite hover_ExitGameButton;
    public Sprite normal_NewGameButton;
    public Sprite hover_NewGameButton;

    public TMP_Text finalScore;
    public TMP_Text dayText;
    public TMP_Text hourText;

    private Color normalColor = ColorsPalette.ButtonsColors.normalColor;
    private Color hoverColor = ColorsPalette.ButtonsColors.hoverColor;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void Start()
    {
        if (saveSystem == null)
            Debug.Log("Save system script not found");
    }

    private void OnEnable()
    {
        if(saveButton != null && exitButton != null)
        {
            saveButton.sprite = normalSave_Sprite;
            exitButton.sprite = normalExit_Sprite;

            saveText.color = normalColor;
            exitText.color = normalColor;
        }
    }

    public void OnSaveButtonEnter()
    {
        if(saveButton != null)
        {
            saveButton.sprite = hoverSave_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if(saveText != null)
            saveText.color = hoverColor;
    }

    public void OnSaveButtonExit()
    {
        if (saveButton != null)
            saveButton.sprite = normalSave_Sprite;
        if (saveText != null)
            saveText.color = normalColor;
    }
    public void OnExitButtonEnter()
    {
        if (exitButton != null)
        {
            exitButton.sprite = hoverExitSprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (exitText != null)
            exitText.color = hoverColor;
    }

    public void OnExitButtonExit()
    {
        if (exitButton != null)
            exitButton.sprite = normalSave_Sprite;
        if (exitText != null)
            exitText.color = normalColor;
    }

    public void SaveGameScore()
    {
        saveSystem.SaveNewScore(scoreSystem.Score);
    }

    public void OnNewGameButtonEnter()
    {
        if (newGameButton != null)
            newGameButton.sprite = hover_NewGameButton;
    }
    public void OnNewGameButtonExit()
    {
        if (newGameButton != null)
            newGameButton.sprite = normal_NewGameButton;
    }
    public void OnStaticsButtonEnter()
    {
        if (staticsButton != null)
            staticsButton.sprite = hover_StaticsButton;
    }
    public void OnStaticsButtonExit()
    {
        if (staticsButton != null)
            staticsButton.sprite = normal_StaticsButton;
    }
    public void OnExitGameButtonEnter()
    {
        if (exitGameButton != null)
            exitGameButton.sprite = hover_NewGameButton;
    }
    public void OnExitGameButtonExit()
    {
        if (exitGameButton != null)
            exitGameButton.sprite = normal_NewGameButton;
    }

    public void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonPressed);
    }

    public void ShowFinalStats()
    {
        dayText.text = DateTime.Now.ToString("dd/MM/yy");
        hourText.text = DateTime.Now.ToString("HH:mm");
        finalScore.text = scoreSystem.Score.ToString();
    }
}
