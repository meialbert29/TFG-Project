using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonsController : MonoBehaviour
{
    public Image startButton;
    public Image settingsButton;
    public Image stadisticsButton;

    public Sprite hoverMain_Sprite;
    public Sprite normalMain_Sprite;

    public TMP_Text startText;
    public TMP_Text settingsText;
    public TMP_Text stadisticsText;
    private Color normalColor = ColorsPalette.ButtonsColors.normalColor;
    private Color hoverColor = ColorsPalette.ButtonsColors.hoverColor;

    public void OnStartButtonEnter()
    {
        if (startButton != null)
            startButton.sprite = hoverMain_Sprite;
        if (startText != null)
            startText.color = hoverColor;
    }
    public void OnStartButtonExit()
    {
        if (startButton != null)
            startButton.sprite = normalMain_Sprite;
        if (startButton != null)
            startText.color = normalColor;
    }

    public void OnSettingstButtonEnter()
    {
        if (settingsButton != null)
            settingsButton.sprite = hoverMain_Sprite;
        if (settingsText != null)
            settingsText.color = hoverColor;
    }
    public void OnSettingsButtonExit()
    {
        if (settingsButton != null)
            settingsButton.sprite = normalMain_Sprite;
        if (settingsText != null)
            settingsText.color = normalColor;
    }

    public void OnStadisticsButtonEnter()
    {
        if (stadisticsButton != null)
            stadisticsButton.sprite = hoverMain_Sprite;
        if (stadisticsText != null)
            stadisticsText.color = hoverColor;
    }
    public void OnStadisticsButtonExit()
    {
        if (stadisticsButton != null)
            stadisticsButton.sprite = normalMain_Sprite;
        if (stadisticsText != null)
            stadisticsText.color = normalColor;
    }
}
