using System;
using UnityEditor;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    private VegetationController _vegetationController;
    private RainController _rainController;
    public GeneralController _generalController;

    // properties
    private Material cloudsMaterial;
    private Vector4 _rotateProjection;
    private float _noiseScale;
    private float _cloudsSpeed;
    private float _noiseHeight;
    private Vector3 _noiseRemap;
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

    float _current_NoiseEdge1;
    float _current_NoiseScale;
    float _current_fresnelOpacity;

    // state variables
    private string _moodType;

    // color variables
    private Color _current_ValleyColor;
    private Color _current_PeakColor;

    private Color sadPeak = ColorsPalette.CloudsColors.sadPeak;
    private Color sadValley = ColorsPalette.CloudsColors.sadValley;

    // time variables
    private float _transitionProgress;
    private float _transitionDuration;
    private bool _skyChanging;

    // target variables
    float target_NoiseScale;
    float target_NoiseEdge1;
    float target_fresnelOpacity;
    Color targetValley = Color.white;
    Color targetPeak = Color.white;


    void Start()
    {
        //_gc = FindAnyObjectByType<GeneralController>();
        cloudsMaterial = GetComponent<Renderer>().material;
        
        if(cloudsMaterial == null || _generalController == null)
        {
            Debug.Log("Error in Clouds Controller");
            return;
        }
        else
        {
            // get default settings from original material
            _rotateProjection = cloudsMaterial.GetVector("_RotateProjection");
            _noiseScale = cloudsMaterial.GetFloat("_NoiseScale");
            _cloudsSpeed = cloudsMaterial.GetFloat("_CloudSpeed");
            _noiseHeight = cloudsMaterial.GetFloat("_NoiseHeight");
            _noiseRemap = cloudsMaterial.GetVector("_NoiseRemap");
            _valleyColor = cloudsMaterial.GetColor("_ColorValley");
            _peakColor = cloudsMaterial.GetColor("_ColorPeak");
            _noiseEdge1 = cloudsMaterial.GetFloat("_NoiseEdge1");
            _noiseEdge2 = cloudsMaterial.GetFloat("_NoiseEdge2");
            _noisePower = cloudsMaterial.GetFloat("_NoisePower");
            _baseScale = cloudsMaterial.GetFloat("_BaseScale");
            _baseSpeed = cloudsMaterial.GetFloat("_BaseSpeed");
            _baseStrength = cloudsMaterial.GetFloat("_BaseStrength");
            _emissionStrength = cloudsMaterial.GetFloat("_EmissionStrength");
            _curvatureRadius = cloudsMaterial.GetFloat("_CurvatureRadius");
            _fresnelPower = cloudsMaterial.GetFloat("_FresnelPower");
            _fresnelOpacity = cloudsMaterial.GetFloat("_FresnelOpacity");
            _fadeDepth = cloudsMaterial.GetFloat("_FadeDepth");

            _transitionDuration = 10f;
            _skyChanging = false;
        }

        _vegetationController = FindAnyObjectByType<VegetationController>();
        _rainController = FindAnyObjectByType<RainController>();

        _current_ValleyColor = _valleyColor;
        _current_PeakColor = _peakColor;
        _current_NoiseEdge1 = _noiseEdge1;
        _current_NoiseScale = _noiseScale;
        _current_fresnelOpacity = _fresnelOpacity;
    }

    // Update is called once per frame
    private bool _rainStarted = false;

    void Update()
    {
        if (_generalController.MoodChanging)
        {
            if (!_skyChanging)
            {
                _moodType = _generalController.Mood;
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
                target_NoiseScale = _noiseScale;
                target_NoiseEdge1 = _noiseEdge1;
                target_fresnelOpacity = _fresnelOpacity;
                targetValley = _valleyColor;
                targetPeak = _peakColor;
                break;

            case "sad":
                target_NoiseScale = 0.31f;
                target_NoiseEdge1 = 0.06f;
                target_fresnelOpacity = 0.18f;
                targetValley = sadValley;
                targetPeak= sadPeak;
                break;
            case "stressed":
                break;
            
        }

        ChangeCloudSettingsOverTime(target_NoiseScale, target_NoiseEdge1, target_fresnelOpacity, targetValley, targetPeak);
    }

    private void ChangeCloudSettingsOverTime(float noiseScale, float noiseEdge1, float fresnelOpacity, Color targetValleyColor, Color targetPeakColor)
    {
        if(_transitionProgress < 1f)
        {
            _current_NoiseEdge1 = Mathf.Lerp(_current_NoiseEdge1, noiseEdge1, _transitionProgress);
            _current_NoiseScale = Mathf.Lerp(_current_NoiseScale, noiseScale, _transitionProgress);
            _current_fresnelOpacity = Mathf.Lerp(_current_fresnelOpacity, fresnelOpacity, _transitionProgress);

            _current_ValleyColor = Color.Lerp(_current_ValleyColor, targetValleyColor, _transitionProgress);
            _current_PeakColor = Color.Lerp(_current_PeakColor, targetPeakColor, _transitionProgress);
        }

        ApplyCloudSettings();
    }

    private void UpdateStartCloudSettings()
    {
        _current_NoiseScale = target_NoiseScale;
        _current_NoiseEdge1 = target_NoiseEdge1;
        _current_fresnelOpacity = target_fresnelOpacity;
        _current_ValleyColor = targetValley;
        _current_PeakColor = targetPeak;
    }

    private void ApplyCloudSettings()
    {
        cloudsMaterial.SetFloat("_NoiseScale", _current_NoiseScale);
        cloudsMaterial.SetFloat("_NoiseEdge1", _current_NoiseEdge1);
        cloudsMaterial.SetFloat("_FresnelOpacity", _current_fresnelOpacity);
        cloudsMaterial.SetColor("_ColorValley", _current_ValleyColor);
        cloudsMaterial.SetColor("_ColorPeak", _current_PeakColor);
    }
}
