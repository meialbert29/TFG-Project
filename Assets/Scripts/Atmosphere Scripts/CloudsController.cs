using System;
using UnityEditor;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    [SerializeField] private VegetationController _vegetationController;
    [SerializeField] private RainController _rainController;
    [SerializeField] private GeneralController _generalController;

    // properties
    private Material cloudsMaterial;
    private Vector4 _rotateProjection;
    private float _noiseScale;
    private float _cloudsSpeed;
    private float _noiseHeight;
    private Vector4 _noiseRemap;
    private Color _valleyColor;
    private Color _peakColor;
    private float _noiseEdge1;
    private float _noiseEdge2;
    private float _noisePower;
    private float _baseScale;
    private float _baseSpeed;
    private float _baseStrength;
    private float _emissionStrength;
    private float _curvatureRadius;
    private float _fresnelPower;
    private float _fresnelOpacity;
    private float _fadeDepth;

    private float _current_NoiseScale;
    private float _current_CloudSpeed;
    private float _current_NoiseHeight;
    private Vector4 _current_NoiseRemap;
    private Color _current_ValleyColor;
    private Color _current_PeakColor;
    private float _current_NoiseEdge1;
    private float _current_NoiseEdge2;
    private float _current_NoisePower;
    private float _current_BaseScale;
    private float _current_BaseSpeed;
    private float _current_BaseStrength;
    private float _current_EmissionStrenght;
    private float _current_CurvatureRadius;
    private float _current_FresnelPower;
    private float _current_FresnelOpacity;
    private float _current_FadeDepth;

    // state variables
    private string _moodType;
    private string _previousMoodType = "";

    // color variables
    private Color sadValley = ColorsPalette.CloudsColors.sadValley;
    private Color sadPeak = ColorsPalette.CloudsColors.sadPeak;

    private Color stressedValley = ColorsPalette.CloudsColors.stressedValley;
    private Color stressedPeak = ColorsPalette.CloudsColors.stressedPeak;

    private Color anxiousValley = ColorsPalette.CloudsColors.anxiousValley;
    private Color anxiousPeak = ColorsPalette.CloudsColors.anxiousPeak;

    // time variables
    private float _transitionProgress;
    private float _transitionDuration;
    private bool _skyChanging;
    private bool _rainStarted = false;

    // target variables
    float target_NoiseScale;
    float target_CloudSpeed;
    float target_NoiseHeight;
    Vector4 target_NoiseRemap;
    float target_NoiseEdge1;
    float target_NoiseEdge2;
    float target_NoisePower;
    float target_BaseScale;
    float target_BaseSpeed;
    float target_BaseStrength;
    float target_EmissionStrength;
    float target_CurvatureRadious;
    float target_FresnelPower;
    float target_FresnelOpacity;
    float target_FadeDepth;
    Color target_Valley = Color.white;
    Color target_Peak = Color.white;

    void Start()
    {
        cloudsMaterial = GetComponent<Renderer>().material;


        if(IsMissing(_rainController, nameof(_rainController)) ||
           IsMissing(_vegetationController, nameof(_vegetationController)) ||
           IsMissing(_generalController, nameof(_generalController)) ||
           IsMissing(cloudsMaterial, nameof(cloudsMaterial))
        )
            return;
        
        var instanceMaterial = GetComponent<Renderer>().sharedMaterial;   

        // get default settings from original material
        _rotateProjection = instanceMaterial.GetVector("_RotateProjection");
        _noiseScale = instanceMaterial.GetFloat("_NoiseScale");
        _cloudsSpeed = instanceMaterial.GetFloat("_CloudSpeed");
        _noiseHeight = instanceMaterial.GetFloat("_NoiseHeight");

        _noiseRemap = instanceMaterial.GetVector("_NoiseRemap");

        _valleyColor = instanceMaterial.GetColor("_ColorValley");
        _peakColor = instanceMaterial.GetColor("_ColorPeak");
        _noiseEdge1 = instanceMaterial.GetFloat("_NoiseEdge1");
        _noiseEdge2 = instanceMaterial.GetFloat("_NoiseEdge2");
        _noisePower = instanceMaterial.GetFloat("_NoisePower");
        _baseScale = instanceMaterial.GetFloat("_BaseScale");
        _baseSpeed = instanceMaterial.GetFloat("_BaseSpeed");
        _baseStrength = cloudsMaterial.GetFloat("_BaseStrength");
        _emissionStrength = instanceMaterial.GetFloat("_EmissionStrength");
        _curvatureRadius = cloudsMaterial.GetFloat("_CurvatureRadius");
        _fresnelPower = instanceMaterial.GetFloat("_FresnelPower");
        _fresnelOpacity = cloudsMaterial.GetFloat("_FresnelOpacity");
        _fadeDepth = instanceMaterial.GetFloat("_FadeDepth");

        _transitionDuration = 5f;
        _skyChanging = false;
        
        _current_NoiseEdge1 = _noiseEdge1;
        _current_NoiseEdge2 = _noiseEdge2;
        _current_NoiseScale = _noiseScale;
        _current_NoiseRemap = _noiseRemap;
        _current_BaseScale = _baseScale;
        _current_BaseSpeed = _baseSpeed;
        _current_FresnelOpacity = _fresnelOpacity;
        _current_FresnelPower = _fresnelPower;
        _current_FadeDepth = _fadeDepth;
        _current_CloudSpeed = _cloudsSpeed;
        _current_ValleyColor = _valleyColor;
        _current_PeakColor = _peakColor;
    }

    private bool IsMissing<T>(T obj, string name) where T : class
    {
        if (obj == null)
        {
            Debug.LogError($"{name} is missing in CloudsController.");
            return true;
        }
        return false;
    }


    // Update is called once per frame
    void Update()
    {
        if (_generalController.MoodChanging)
        {
            string newMood = _generalController.Mood;

            if (!_skyChanging || newMood != _moodType)
            {
                // si el estado cambió durante una transición: reiniciar desde valores actuales
                _current_NoiseScale = Mathf.Lerp(_current_NoiseScale, target_NoiseScale, _transitionProgress);
                _current_NoiseEdge1 = Mathf.Lerp(_current_NoiseEdge1, target_NoiseEdge1, _transitionProgress);
                _current_FresnelOpacity = Mathf.Lerp(_current_FresnelOpacity, target_FresnelOpacity, _transitionProgress);
                _current_ValleyColor = Color.Lerp(_current_ValleyColor, target_Valley, _transitionProgress);
                _current_PeakColor = Color.Lerp(_current_PeakColor, target_Peak, _transitionProgress);

                // iniciar nueva transición
                _moodType = newMood;
                _transitionProgress = 0f;
                _skyChanging = true;
                _rainStarted = false;
            }
        }

        if (_skyChanging)
        {
            _transitionProgress += Time.deltaTime / _transitionDuration;
            ChangeCloudsSettings(_moodType);

            if (_transitionProgress >= 0.2f && !_rainStarted)
            {
                _rainController.RainStart();
                _rainStarted = true;
            }

            if (_transitionProgress >= 1f)
            {
                UpdateStartCloudSettings();
                _transitionProgress = 0f;
                _skyChanging = false;
            }
        }
    }

    private void ChangeCloudsSettings(string mood)
    {   
        switch (mood)
        {
            case "neutral":
                target_NoiseEdge1 = _noiseEdge1;
                target_NoiseEdge2 = _noiseEdge2;
                target_NoiseScale = _noiseScale;
                target_NoisePower = _noisePower;    
                target_NoiseRemap = _noiseRemap;
                target_BaseScale = _baseScale;
                target_BaseSpeed = _baseSpeed;
                target_FresnelOpacity = _fresnelOpacity;
                target_FresnelPower = _fresnelPower;
                target_FadeDepth = _fadeDepth;
                target_CloudSpeed = _cloudsSpeed;
                target_Valley = _valleyColor;
                target_Peak = _peakColor;
                break;

            case "sad":
                target_NoiseScale = 0.31f;
                target_NoiseEdge1 = 0.06f;
                target_NoiseEdge2 = _noiseEdge2;

                target_FresnelOpacity = 0.18f;
                target_Valley = sadValley;
                target_Peak= sadPeak;

                target_FadeDepth = _fadeDepth;
                target_CloudSpeed = _cloudsSpeed;
                target_NoiseRemap = _noiseRemap;
                target_BaseScale = _baseScale;
                target_BaseSpeed = _baseSpeed;


                break;

            case "stressed":

                target_NoiseRemap = _noiseRemap;
                target_NoiseScale = 0.68f;
                target_NoiseEdge1 = 0.96f;
                target_NoiseEdge2 = 0f;
                target_NoiseRemap.x = -0.12f;
                target_NoiseEdge1 = 0.01f;

                target_FresnelOpacity = _fresnelOpacity;
                target_FresnelPower = _fresnelPower;
                target_FadeDepth = _fadeDepth;

                target_CloudSpeed = 5.44f;


                target_NoisePower = 0.73f;
                target_BaseScale = -0.23f;
                target_BaseSpeed = 10f;

                target_Valley = stressedValley;
                target_Peak = stressedPeak;

                break;

            case "calm":
                target_NoiseScale = _noiseScale;
                target_NoiseRemap = _noiseRemap;
                target_NoisePower = _noisePower;
                target_FresnelPower = _fresnelPower;
                target_FadeDepth = _fadeDepth;
                target_NoiseEdge1 = _noiseEdge1;
                target_NoiseEdge2 = _noiseEdge2;

                target_NoiseRemap.w = 5f;
                target_BaseScale = 0.07f;
                target_BaseStrength = -0.12f;
                target_FresnelOpacity = 0.44f;
                target_Valley = _valleyColor;
                target_Peak = _peakColor;

                break;

            case "anxious":
                target_BaseStrength = _baseStrength;
                target_NoiseRemap = _noiseRemap;

                target_NoiseScale = -0.21f;
                target_CloudSpeed = 20f;
                target_NoiseRemap.y = 1.49f;
                target_NoiseRemap.z = -1.7f;
                target_NoiseEdge1 = 0.57f;
                target_NoiseEdge2 = 0.73f;
                target_BaseScale = -0.12f;
                target_BaseSpeed = 0.49f;

                target_FresnelPower = 34.8f;
                target_FresnelOpacity = 0.46f;
                target_FadeDepth = 72.5f;
                target_Valley = anxiousValley;
                target_Peak = anxiousPeak;

                break;
            
        }

        ChangeCloudSettingsOverTime(target_NoiseScale, target_NoiseEdge1, target_NoiseEdge2, target_NoisePower, target_BaseScale,
                                    target_BaseSpeed, target_BaseStrength, target_FresnelOpacity, target_CloudSpeed, target_NoiseRemap, target_Valley, target_Peak);
    }

    private void ChangeCloudSettingsOverTime(float noiseScale, float noiseEdge1, float noiseEdge2, float noisePower, float baseScale, float baseSpeed,
                                             float baseStrength,float fresnelOpacity, float cloudsSpeed, Vector4 noiseRemap, Color targetValleyColor, Color targetPeakColor)
    {
        if(_transitionProgress < 1f)
        {
            _current_NoiseScale = Mathf.Lerp(_current_NoiseScale, noiseScale, _transitionProgress);
            _current_NoiseEdge1 = Mathf.Lerp(_current_NoiseEdge1, noiseEdge1, _transitionProgress);
            _current_NoiseEdge2 = Mathf.Lerp(_current_NoiseEdge2, noiseEdge2, _transitionProgress);
            _current_NoisePower = Mathf.Lerp(_current_NoisePower, noisePower, _transitionProgress);

            _current_BaseScale = Mathf.Lerp(_current_BaseScale, baseScale, _transitionProgress);
            _current_BaseSpeed = Mathf.Lerp(_current_BaseSpeed, baseSpeed, _transitionProgress);
            _current_BaseStrength = Mathf.Lerp(_current_BaseStrength, baseStrength, _transitionProgress);
            
            _current_FresnelOpacity = Mathf.Lerp(_current_FresnelOpacity, fresnelOpacity, _transitionProgress);
            _current_CloudSpeed = Mathf.Lerp(_current_CloudSpeed, cloudsSpeed, _transitionProgress);

            _current_NoiseRemap.x = Mathf.Lerp(_current_NoiseRemap.x, noiseRemap.x, _transitionProgress);
            //_current_NoiseRemap.y = Mathf.Lerp(_current_NoiseRemap.y, noiseRemap.y, _transitionProgress);
            //_current_NoiseRemap.z = Mathf.Lerp(_current_NoiseRemap.z, noiseRemap.z, _transitionProgress);
            _current_NoiseRemap.w = Mathf.Lerp(_current_NoiseRemap.w, noiseRemap.w, _transitionProgress);

            _current_ValleyColor = Color.Lerp(_current_ValleyColor, targetValleyColor, _transitionProgress);
            _current_PeakColor = Color.Lerp(_current_PeakColor, targetPeakColor, _transitionProgress);
        }

        ApplyCloudSettings();
    }

    private void UpdateStartCloudSettings()
    {
        _current_NoiseScale = target_NoiseScale;
        _current_NoiseEdge1 = target_NoiseEdge1;
        _current_FresnelOpacity = target_FresnelOpacity;
        _current_ValleyColor = target_Valley;
        _current_PeakColor = target_Peak;
    }

    private void ApplyCloudSettings()
    {
        cloudsMaterial.SetFloat("_NoiseScale", _current_NoiseScale);
        cloudsMaterial.SetFloat("_NoiseEdge1", _current_NoiseEdge1);
        cloudsMaterial.SetFloat("_NoiseEdge2", _current_NoiseEdge2);
        cloudsMaterial.SetFloat("_NoisePower", _current_NoisePower);
        cloudsMaterial.SetFloat("_BaseScale", _current_BaseScale);
        cloudsMaterial.SetFloat("_BaseSpeed", _current_BaseSpeed);
        cloudsMaterial.SetFloat("_FresnelOpacity", _current_FresnelOpacity);
        cloudsMaterial.SetFloat("_CloudSpeed", _current_CloudSpeed);
        cloudsMaterial.SetVector("_NoiseRemap", _current_NoiseRemap);
        cloudsMaterial.SetColor("_ColorValley", _current_ValleyColor);
        cloudsMaterial.SetColor("_ColorPeak", _current_PeakColor);
    }
}
