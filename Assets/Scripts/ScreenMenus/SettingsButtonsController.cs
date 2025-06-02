using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
public class SettingsButtonsController : MonoBehaviour
{
    AudioManager audioManager;

    // images variables
    public Image saveButton;
    public TMP_Text saveText;
    // sprites variables
    public Sprite normal_Sprite;
    public Sprite hover_Sprite;

    private Color normalColor = ColorsPalette.ButtonsColors.normalColor;
    private Color hoverColor = ColorsPalette.ButtonsColors.hoverColor;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void OnEnable()
    {
        if (saveButton != null)
        {
            saveButton.sprite = normal_Sprite;
            saveText.color = normalColor;
        }
    }

    public void OnSaveButtonEnter()
    {
        if (saveButton != null)
        {
            saveButton.sprite = hover_Sprite;
            audioManager.PlaySFX(audioManager.buttonPressed);
        }
            
        if (saveText != null)
            saveText.color = hoverColor;
    }
    public void OnSaveButtonExit()
    {
        if (saveButton != null)
            saveButton.sprite = normal_Sprite;
        if (saveText != null)
            saveText.color = normalColor;
    }
    public void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonHover);
    }
}
