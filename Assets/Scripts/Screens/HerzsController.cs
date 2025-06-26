using Assets.LSL4Unity.Scripts.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HerzsController : MonoBehaviour
{
    [SerializeField] private ExampleFloatInlet museController;
    public Slider slider;        
    public TMP_Text frequencyText;
    public float currentFrequency; 
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetFrequency(float hz)
    {
        currentFrequency = hz;
        slider.value = currentFrequency;
        if (frequencyText != null)
            frequencyText.text = currentFrequency.ToString() + " Hz";
    }
}
