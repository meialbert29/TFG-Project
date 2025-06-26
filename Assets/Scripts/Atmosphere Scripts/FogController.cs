using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{

    private bool enableFog = false; // controls whether the fog is enabled or disabled.
    private Color fogColor;
    private float fogDensity = 0.02f; // fog density (adjust as needed).
    private string state = "neutral";

    private Coroutine fogTransitionCoroutine;

    void Start()
    {
        // initially, set the fog to be disabled.
        RenderSettings.fog = false;
    }

    void Update()
    {
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
        }
    }

    public void ToggleFog(bool isEnabled)
    {
        enableFog = isEnabled;
    }

    public void DisableFog()
    {
        RenderSettings.fog = false;
        enableFog = false;
    }

    public void ChangeFogSettings(bool enable, Color newColor, float newDensity)
    {
        if (fogTransitionCoroutine != null)
            StopCoroutine(fogTransitionCoroutine);

        fogTransitionCoroutine = StartCoroutine(TransitionFog(enable, newColor, newDensity, 2f));
    }

    private IEnumerator TransitionFog(bool enable, Color targetColor, float targetDensity, float duration)
    {
        float time = 0f;
        Color startColor = RenderSettings.fogColor;
        float startDensity = RenderSettings.fogDensity;

        if (enable) RenderSettings.fog = true;

        while (time < duration)
        {
            float t = time / duration;
            fogColor = Color.Lerp(startColor, targetColor, t);
            fogDensity = Mathf.Lerp(startDensity, targetDensity, t);

            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;

            time += Time.deltaTime;
            yield return null;
        }

        fogColor = targetColor;
        fogDensity = targetDensity;

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        RenderSettings.fog = enable;
        enableFog = enable;
    }

}
