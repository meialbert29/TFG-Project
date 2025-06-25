using Assets.LSL4Unity.Scripts.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HerzsController : MonoBehaviour
{
    [SerializeField] private ExampleFloatInlet museController;
    public Slider slider;          // Referencia al Slider UI
    public TMP_Text frequencyText;     // Referencia al texto opcional
    public float currentFrequency; // Este valor lo actualizarás desde tus datos en tiempo real
    
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
