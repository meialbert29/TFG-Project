using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class HelpMenuController : MonoBehaviour
{
    AudioManager audioManager;

    public Image backButton;

    public Sprite normalBack_Sprite;
    public Sprite hoverBack_Sprite;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnEnable()
    {
        // Reset normal sprite
        if (backButton != null)
            backButton.sprite = normalBack_Sprite;
    }
    public void ButtonSound()
    {
        audioManager.PlaySFX(audioManager.buttonPressed);
    }
    public void OnBackButtonEnter()
    {
        if(backButton != null)
        {
            backButton.sprite = hoverBack_Sprite;
            audioManager.PlaySFX(audioManager.buttonHover);
        }
    }
    public void OnBackButtonExit()
    {
        if (backButton != null)
            backButton.sprite = normalBack_Sprite;
    }
}
