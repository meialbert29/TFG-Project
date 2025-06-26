using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] GeneralController generalController;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource rainSource;

    [Header("Audio Clip")]
    public AudioClip musicBackground;
    public AudioClip mainThemeBackground;
    public AudioClip buttonHover;
    public AudioClip buttonPressed;
    public AudioClip error;

    [Header("Music")]
    public AudioClip neutralMusic;
    public AudioClip sadMusic;
    public AudioClip calmMusic;
    public AudioClip stressMusic;
    public AudioClip anxiousMusic;

    [Header("SFX")]
    public AudioClip softRain;
    public AudioClip normalRain;
    public AudioClip neutralWind;
    public AudioClip calmlWind;
    public AudioClip stressWind;
    public AudioClip anxiousWind;

    private const string volumeMusicParam = "Music";
    private const string volumeSFXParam = "SFX";

    private bool isSFXFading = false;

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "GameScene")
        {
            musicSource.clip = neutralMusic;
            SFXSource.clip = neutralWind;
            rainSource.clip = null;
        }
        else
        {
            musicSource.clip = mainThemeBackground;
            SFXSource.clip = null;
            rainSource.clip = null;
        }

        musicSource.Play();
        if (SFXSource.clip != null) SFXSource.Play();
        if (rainSource.clip != null) rainSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void ChangeMusicWithMixerFade(AudioClip newMusicClip, float duration = 1.5f)
    {
        StartCoroutine(FadeOutInMusic(newMusicClip, duration));
    }

    private IEnumerator FadeOutInMusic(AudioClip newMusicClip, float duration)
    {
        float currentTime = 0f;
        audioMixer.GetFloat(volumeMusicParam, out float currentVolume);
        float startVolume = currentVolume;
        float targetVolume = -80f;

        // fade out
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            audioMixer.SetFloat(volumeMusicParam, newVolume);
            yield return null;
        }

        audioMixer.SetFloat(volumeMusicParam, targetVolume);
        musicSource.Stop();
        musicSource.clip = newMusicClip;
        musicSource.Play();

        // fade in
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(targetVolume, startVolume, currentTime / duration);
            audioMixer.SetFloat(volumeMusicParam, newVolume);
            yield return null;
        }

        audioMixer.SetFloat(volumeMusicParam, startVolume);
    }

    public void ChangeSFXWithMixerFade(AudioClip newWindClip, AudioClip newRainClip, float duration = 1.5f)
    {
        if (!isSFXFading)
        {
            StartCoroutine(FadeOutInSFX(newWindClip, duration));
            StartCoroutine(FadeOutInSFX(newRainClip, duration));
        }
    }

    private IEnumerator FadeOutInSFX(AudioClip newSFXClip, float duration)
    {
        isSFXFading = true;

        float currentTime = 0f;
        float startVolume = 0f;
        if (!audioMixer.GetFloat(volumeSFXParam, out startVolume))
            startVolume = 0f;

        float targetVolume = -80f;

        // Fade Out
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            audioMixer.SetFloat(volumeSFXParam, newVolume);
            yield return null;
        }

        audioMixer.SetFloat(volumeSFXParam, targetVolume);
        SFXSource.Stop();
        SFXSource.clip = newSFXClip;
        SFXSource.loop = true;
        SFXSource.Play();

        // Fade In
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(targetVolume, startVolume, currentTime / duration);
            audioMixer.SetFloat(volumeSFXParam, newVolume);
            yield return null;
        }

        audioMixer.SetFloat(volumeSFXParam, startVolume);
        isSFXFading = false;
    }

    //public void ChangeSFXClips(AudioClip newWindClip, AudioClip newRainClip)
    //{
    //    if (newWindClip != null)
    //    {
    //        SFXSource.clip = newWindClip;
    //        SFXSource.loop = true;
    //        SFXSource.Play();
    //    }
    //    else
    //    {
    //        SFXSource.Stop();
    //        SFXSource.clip = null;
    //    }

    //    if (newRainClip != null)
    //    {
    //        rainSource.clip = newRainClip;
    //        rainSource.loop = true;
    //        rainSource.Play();
    //    }
    //    else
    //    {
    //        rainSource.Stop();
    //        rainSource.clip = null;
    //    }
    //}
}
