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

    private Color sadPeak = new Color(0.3602704f, 0.4823402f, 0.553459f, 1f);
    private Color sadValley = new Color(0.5847869f, 0.7623752f, 0.8050313f, 1f);

    // time variables
    private float _transitionProgress;
    private float _transitionDuration;
    private bool _skyChanging;


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
    }

    // Update is called once per frame
    private bool _rainStarted = false;

    void Update()
    {
        if (_generalController.MoodChanging)
        {
            if (!_skyChanging) // solo la primera vez
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
            changeCloudsSettings();

            if (_transitionProgress >= 0.2f && !_rainStarted)
            {
                _rainController.rainStart();
                _rainStarted = true;
            }

            if (_transitionProgress >= 1f)
            {
                _transitionProgress = 0f;
                _skyChanging = false;
            }
        }
    }


    void changeCloudsSettings()
    {
        if(_moodType == "sad")
        {
            _noiseScale = 0.31f;
            _noiseEdge1 = 0.06f;
            _fresnelOpacity = 0.18f;
        }

        changeCloudSettingsOverTime(_noiseScale, _noiseEdge1, _fresnelOpacity, sadValley, sadPeak);
    }

    void changeCloudSettingsOverTime(float noiseScale, float noiseEdge1, float fresnelOpacity, Color targetValleyColor, Color targetPeakColor)
    {
        if(_transitionProgress < 1f)
        {
            _current_NoiseEdge1 = Mathf.Lerp(cloudsMaterial.GetFloat("_NoiseEdge1"), _noiseEdge1, _transitionProgress);
            _current_NoiseScale = Mathf.Lerp(cloudsMaterial.GetFloat("_NoiseScale"), _noiseScale, _transitionProgress);
            _current_fresnelOpacity = Mathf.Lerp(cloudsMaterial.GetFloat("_FresnelOpacity"), _fresnelOpacity, _transitionProgress);

            _current_ValleyColor = Color.Lerp(cloudsMaterial.GetColor("_ColorValley"), targetValleyColor, _transitionProgress);
            _current_PeakColor = Color.Lerp(cloudsMaterial.GetColor("_ColorPeak"), targetPeakColor, _transitionProgress);
        }

        cloudsMaterial.SetFloat("_NoiseScale", _current_NoiseScale);
        cloudsMaterial.SetFloat("_NoiseEdge1", _current_NoiseEdge1);
        cloudsMaterial.SetFloat("_FresnelOpacity", _current_fresnelOpacity);
        cloudsMaterial.SetColor("_ColorValley", _current_ValleyColor);
        cloudsMaterial.SetColor("_ColorPeak", _current_PeakColor);
    }

    public bool skyIsChanging()
    {
        return _skyChanging;
    }
}
