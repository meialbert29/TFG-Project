using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
public class ModeMenu : MonoBehaviour
{
    AudioManager audioManager;

    [SerializeField] private Image manualButton;
    [SerializeField] private Image neuronalButton;

    [SerializeField] private Sprite hoverManual_Sprite;
    [SerializeField] private Sprite normalManual_Sprite;
    [SerializeField] private Sprite hoverNeuronal_Sprite;
    [SerializeField] private Sprite normalNeuronal_Sprite;

    [SerializeField] private TMP_Text manualText;
    [SerializeField] private TMP_Text neuronalBandText;

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
        if (manualButton != null)
        {
            manualButton.sprite = normalManual_Sprite;
            manualText.color = normalColor;
        }

        if (neuronalButton != null)
        {
            neuronalButton.sprite = normalNeuronal_Sprite;
            neuronalBandText.color = normalColor;
        }
    }

    public void OnManualButtonEnter()
    {
        if (manualButton != null)
        {
            manualButton.sprite = hoverManual_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (manualText != null)
            manualText.color = hoverColor;
    }
    public void OnManualButtonExit()
    {
        if (manualButton != null)
            manualButton.sprite = normalManual_Sprite;
        if (manualText != null)
            manualText.color = normalColor;
    }

    public void OnNeuronalButtonEnter()
    {
        if (neuronalButton != null)
        {
            neuronalButton.sprite = hoverNeuronal_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }

        if (neuronalBandText != null)
            neuronalBandText.color = hoverColor;
    }
    public void OnNeuronalButtonExit()
    {
        if (neuronalButton != null)
            neuronalButton.sprite = normalNeuronal_Sprite;
        if (neuronalBandText != null)
            neuronalBandText.color = normalColor;
    }
}
