using UnityEngine;

public class SunController : MonoBehaviour
{
    private Color neutral = new Color(0.6918238f, 0.6847792f, 0.6548395f, 0.5843138f);
    private Color sad = new Color(0.3300502f, 0.4164391f, 0.7044024f, 0.5f);
    private Color calm = new Color(0.2819204f, 0.5220125f, 0.2084766f, 0.5f);
    private Color stress = new Color(0.4923722f, 0.3250464f, 0.5974842f, 0.5f);
    private Color anxiety = new Color(0f, 0.17541f, 0.3522012f, 0.5f);

    [SerializeField] private Light sunLight;
    [SerializeField] private Color sunColor;
    [Range(0, 5)] // Intensity range
    public float intensity = 1.0f; //Default intensity

    public float transitionDuration = 2f; // Time to complete inteprolation

    private float transitionProgress = 0f;
    private bool automaticModeActivated = false;
    private bool isTransitioning = false;

    void Start()
    {
        if (sunLight == null)
        {
            if (sunLight != null && sunLight.type == LightType.Directional)
            {
                sunColor = neutral;
                sunLight.intensity = intensity;
            }
            else
            {
                Debug.Log("Directional light not found");
            }
        }

        StartTransition(sunColor, intensity);
    }

    // Update is called once per frame
    void Update()
    {
        StartTransition(sunColor, intensity);
    }

    public void StartTransition(Color color, float intensity)
    {
        if (sunLight != null)
        {
            sunLight.color = color;
            sunLight.intensity = intensity;
        }
    }

    private void UpdateSunState(Color color, float intensity)
    {
        transitionProgress = 0f;
        sunColor = color;
        this.intensity = intensity;
        StartTransition(sunColor, intensity);
    }
    public void SetLightState(string state)
    {
        // Lógica para cambiar el estado de la luz dependiendo del estado recibido
        if (state == "sad") UpdateSunState(sad, 0.5f);
        else if (state == "calm") UpdateSunState(calm, 2);
        else if (state == "neutral") UpdateSunState(neutral, 1.5f);
        else if (state == "stress") UpdateSunState(stress, 0.2f);
        else if (state == "anxiety") UpdateSunState(anxiety, 0.2f);
    }
}
