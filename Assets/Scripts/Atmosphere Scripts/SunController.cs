using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField] private Light sunLight;
    [SerializeField] private float sunTemperature;
    [Range(0, 5)] // intensity range
    public float intensity = 1.0f; // default intensity

    public float transitionDuration = 2f; // time to complete inteprolation

    private float transitionProgress = 0f;
    private bool automaticModeActivated = false;
    private bool isTransitioning = false;

    private float currentTemperature;
    private float currentIntensity;

    private float targetTemperature;
    private float targetIntensity;

    void Start()
    {
        if (sunLight == null)
        {
            Debug.Log("Sun light is not assigned.");
            return;
        }

        currentTemperature = sunLight.colorTemperature;
        currentIntensity = sunLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            float t = Mathf.Clamp01(transitionProgress);

            sunLight.colorTemperature = Mathf.Lerp(currentTemperature, targetTemperature, t);
            sunLight.intensity = Mathf.Lerp(currentIntensity, targetIntensity, t);

            if (t >= 1f)
            {
                isTransitioning = false;
                currentTemperature = targetTemperature;
                currentIntensity = targetIntensity;
            }
        }
    }

    public void StartTransition(float temperature, float intensity)
    {
        if (sunLight != null)
        {
            sunLight.colorTemperature = temperature;
            sunLight.intensity = intensity;
        }
    }

    private void UpdateSunState(float temperature, float intensity)
    {
        transitionProgress = 0f;
        this.intensity = intensity;
        StartTransition(temperature, intensity);
    }
    public void SetLightState(string mood)
    {
        switch (mood)
        {
            case "sad":
                targetTemperature = 11047f;
                targetIntensity = 0.5f;
                break;
            case "neutral":
                targetTemperature = 5000f;
                targetIntensity = 1f;
                break;
            case "calm":
                targetTemperature = 3458f;
                targetIntensity = 2f;
                break;
            case "stressed":
                targetTemperature = 15744f;
                targetIntensity = 0.2f;
                break;
            case "anxious":
                targetTemperature = 20000f;
                targetIntensity = 0.2f;
                break;
        }

        // start transition
        currentTemperature = sunLight.colorTemperature;
        currentIntensity = sunLight.intensity;
        transitionProgress = 0f;
        isTransitioning = true;
    }
}
