using System;
using UnityEditor;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    private VegetationBehaviour _vb;
    private RainController _rc;

    // properties
    private Material clouds;
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
        clouds = GetComponent<Renderer>().material;
        if(clouds == null)
        {
            Debug.Log("Clouds material shader not found");
            return;
        }
        else
        {
            // get default settings from original material
            _rotateProjection = clouds.GetVector("_RotateProjection");
            _noiseScale = clouds.GetFloat("_NoiseScale");
            _cloudsSpeed = clouds.GetFloat("_CloudSpeed");
            _noiseHeight = clouds.GetFloat("_NoiseHeight");
            _noiseRemap = clouds.GetVector("_NoiseRemap");
            _valleyColor = clouds.GetColor("_ColorValley");
            _peakColor = clouds.GetColor("_ColorPeak");
            _noiseEdge1 = clouds.GetFloat("_NoiseEdge1");
            _noiseEdge2 = clouds.GetFloat("_NoiseEdge2");
            _noisePower = clouds.GetFloat("_NoisePower");
            _baseScale = clouds.GetFloat("_BaseScale");
            _baseSpeed = clouds.GetFloat("_BaseSpeed");
            _baseStrength = clouds.GetFloat("_BaseStrength");
            _emissionStrength = clouds.GetFloat("_EmissionStrength");
            _curvatureRadius = clouds.GetFloat("_CurvatureRadius");
            _fresnelPower = clouds.GetFloat("_FresnelPower");
            _fresnelOpacity = clouds.GetFloat("_FresnelOpacity");
            _fadeDepth = clouds.GetFloat("_FadeDepth");

            _transitionDuration = 100f;
            _skyChanging = false;
        }

        _vb = FindAnyObjectByType<VegetationBehaviour>();
        _rc = FindAnyObjectByType<RainController>();
    }

    // Update is called once per frame
    private bool _rainStarted = false;

    void Update()
    {
        if (_vb.getMorphingState())
        {
            if (!_skyChanging) // solo la primera vez
            {
                _moodType = _vb.mood;
                _transitionProgress = 0f;
                _skyChanging = true;
                _rainStarted = false;
            }
        }

        if (_skyChanging)
        {
            _transitionProgress += Time.deltaTime / _transitionDuration;
            changeCloudsSettings();

            if (_transitionProgress >= 0.7f && !_rainStarted)
            {
                _rc.rainStart();
                _rainStarted = true;
            }

            if (_transitionProgress >= 1f)
            {
                _transitionProgress = 0f;
                _skyChanging = false;
                _rc.rainStop();
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
            _current_NoiseEdge1 = Mathf.Lerp(clouds.GetFloat("_NoiseEdge1"), _noiseEdge1, _transitionProgress);
            _current_NoiseScale = Mathf.Lerp(clouds.GetFloat("_NoiseScale"), _noiseScale, _transitionProgress);
            _current_fresnelOpacity = Mathf.Lerp(clouds.GetFloat("_FresnelOpacity"), _fresnelOpacity, _transitionProgress);

            _current_ValleyColor = Color.Lerp(clouds.GetColor("_ColorValley"), targetValleyColor, _transitionProgress);
            _current_PeakColor = Color.Lerp(clouds.GetColor("_ColorPeak"), targetPeakColor, _transitionProgress);
        }

        clouds.SetFloat("_NoiseScale", _current_NoiseScale);
        clouds.SetFloat("_NoiseEdge1", _current_NoiseEdge1);
        clouds.SetFloat("_FresnelOpacity", _current_fresnelOpacity);
        clouds.SetColor("_ColorValley", _current_ValleyColor);
        clouds.SetColor("_ColorPeak", _current_PeakColor);
    }

    public bool skyIsChanging()
    {
        return _skyChanging;
    }
}
