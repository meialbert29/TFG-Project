using Assets.LSL4Unity.Scripts.Examples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GuideController : MonoBehaviour
{
    [SerializeField] private ExampleFloatInlet museController;
    [SerializeField] private WavesReader wavesReader;
    [SerializeField] private GeneralController generalController;
    [SerializeField] private Text informationText;

    //Messages
    // calm state
    private readonly string[] alphaMessages = {
    "You're calm and centered. Keep flowing with it!",
    "Peace surrounds you. Enjoy the moment :).",
    "You’re in a state of balance. Breathe and continue.",
    "You`re doing it great! Keep going."
    };
    // neutral state
    private readonly string[] deltaMessages = {
    "You’re in a steady and balanced state. Keep going.",
    "Your brain is calm and grounded. That’s a great place to be ;).",
    "You're in a quiet, stable mental state. Keep flowing like that!"
    };
    // sad state
    private readonly string[] thetaMessages = {
    "A soft stillness is in you. Let your thoughts drift.",
    "You might be reflecting or daydreaming. That’s okay.",
    "You're in a gentle state. Trust what you feel.",
    "Remember: it's okay to not feel okay.",
    "You seem a little down. Try naming things you're grateful",
    "It's okay to feel low. Think of someone who makes you smile.",
    "Melancholy is part of the journey. Place your hand on your heart and just breathe.",
    "Try this: look around and find something that brings you comfort or beauty.",
    "Emotions are just waves. You're not alone. Let's ride this one gently."
    };
    // stressed state
    private readonly string[] betaMessages = {
    "Your mind is active. Let's slow it down: inhale for 4 seconds, hold for 4, exhale for 4. You can do it!",
    "You're focused, but maybe tense. Try relaxing your shoulders.",
    "It’s okay to take a break. Gently place your hand on your chest and feel your breath.",
    "I sense some pressure. Let's pause: count 5 things you can see around you.",
    "Let your jaw soften. Close your eyes for a moment if it helps."
    };
    // anxious state
    private readonly string[] gammaMessages = {
    "You're processing a lot. Don’t forget to rest your mind.",
    "I feel your brain is working hard. Take it slow.",
    "Too much at once? A short break can help clarity.",
    "Even the storms stop at some point. Take your time!",
    "Let every feeling flow. You're just human!",
    "A storm of thoughts detected. Let’s breathe together: in... and out... slowly.",
    "You're carrying a lot. Feel your feet on the ground. You are safe.",
    "So much activity... try placing both hands over your belly and breathe deeply.",
    "Close your eyes for 5 seconds and listen to the silence beneath the noise.",
    "Let go for a second. You don’t need to do it all at once. One step at a time."
    };

    private int gameMode;
    private string lastMood;
    private string currentMood;
    private string lastWave;
    private string currentWave;

    void Start()
    {
        if(wavesReader == null)
        {
            Debug.Log("Wave reader script not found");
            return;
        }

        gameMode = PlayerPrefs.GetInt("GameMode");
        currentMood = generalController.Mood;
        currentWave = wavesReader.CurrentWave;

        if(gameMode == 0)
            ShowMessageByKeynumber(currentMood);
        else
            ShowMessageByCurrentWave(currentWave);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMode == 0)
        {
            currentMood = generalController.Mood;

            if (generalController.MoodChanging && currentMood != lastMood)
            {
                ShowMessageByKeynumber(currentMood);
                lastMood = currentMood;
            }
        }
        else
        {
            currentWave = wavesReader.CurrentWave;
            if(generalController.MoodChanging && currentWave != lastWave)
            {
                ShowMessageByCurrentWave(currentWave);
                lastWave = currentWave;
            }
        }
    }

    private void ShowMessageByCurrentWave(string currentWave)
    {

        if (currentWave != null)
        {
            switch (currentWave)
            {
                case "Alpha":
                    informationText.text = GetRandomMessage(alphaMessages);
                    break;

                case "Delta":
                    informationText.text = GetRandomMessage(deltaMessages);
                    break;

                case "Theta":
                    informationText.text = GetRandomMessage(thetaMessages);
                    break;

                case "Beta":
                    informationText.text = GetRandomMessage(betaMessages);
                    break;

                case "Gamma":
                    informationText.text = GetRandomMessage(gammaMessages);
                    break;
            }
        }
    }

    private void ShowMessageByKeynumber(string mood)
    {
        if (mood != null)
        {
            switch (mood)
            {
                case "calm":
                    informationText.text = GetRandomMessage(alphaMessages);
                    break;

                case "neutral":
                    informationText.text = GetRandomMessage(deltaMessages);
                    break;

                case "sad":
                    informationText.text = GetRandomMessage(thetaMessages);
                    break;

                case "stressed":
                    informationText.text = GetRandomMessage(betaMessages);
                    break;

                case "anxious":
                    informationText.text = GetRandomMessage(gammaMessages);
                    break;
            }
        }
    }

    private string GetRandomMessage(string[] messages)
    {
        return messages[Random.Range(0, messages.Length)];
    }
}
