using System;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect _vfx;
    [SerializeField] private VegetationController _vegetationController;
    [SerializeField] private GeneralController _generalController;

    [SerializeField] private Gradient _leavesGradient;
    [SerializeField] private Gradient _blendGradient;
    [SerializeField] private bool fall;
    [SerializeField] private int moodIndex;
    

    private Color startKey0, startKey1, startKey2;
    private Color targetKey0, targetKey1, targetKey2;
    private Color startKeyBlend0, startKeyBlend1, startKeyBlend2;
    private Color targetKeyBlend0, targetKeyBlend1, targetKeyBlend2;

    private Color _neutral_Key0 = ColorsPalette.LeavesVFXColors.neutral_Key0;
    private Color _neutral_Key1 = ColorsPalette.LeavesVFXColors.neutral_Key1;
    private Color _neutral_Key2 = ColorsPalette.LeavesVFXColors.neutral_Key2;
    private Color _neutral_KeyBlend0 = ColorsPalette.LeavesVFXColors.neutral_KeyBlend0;
    private Color _neutral_KeyBlend1 = ColorsPalette.LeavesVFXColors.neutral_KeyBlend1;
    private Color _neutral_KeyBlend2 = ColorsPalette.LeavesVFXColors.neutral_KeyBlend2;

    private Color _stressed_Key0 = ColorsPalette.LeavesVFXColors.stressed_Key0;
    private Color _stressed_Key1 = ColorsPalette.LeavesVFXColors.stressed_Key1;
    private Color _stressed_Key2 = ColorsPalette.LeavesVFXColors.stressed_Key2;
    private Color _stressed_KeyBlend0 = ColorsPalette.LeavesVFXColors.stressed_KeyBlend0;
    private Color _stressed_KeyBlend1 = ColorsPalette.LeavesVFXColors.stressed_KeyBlend1;
    private Color _stressed_KeyBlend2 = ColorsPalette.LeavesVFXColors.stressed_KeyBlend2;

    private Color _sad_Key0 = ColorsPalette.LeavesVFXColors.sad_Key0;
    private Color _sad_Key1 = ColorsPalette.LeavesVFXColors.sad_Key1;
    private Color _sad_Key2 = ColorsPalette.LeavesVFXColors.sad_Key2;
    private Color _sad_KeyBlend1 = ColorsPalette.LeavesVFXColors.sad_KeyBlend1;
    private Color _sad_KeyBlend2 = ColorsPalette.LeavesVFXColors.sad_KeyBlend2;

    private string moodType;
    private string previousMoodType;

    GradientColorKey[] keys;
    GradientColorKey[] keysBlend;

    // getters & setters
    public bool Fall { get { return fall; } set { fall = value; } }

    void Start()
    {
        _vegetationController = transform.GetComponent<VegetationController>();
        _vfx = GetComponentInChildren<VisualEffect>();

        moodType = _generalController.Mood;
        previousMoodType = moodType;
        moodIndex = 0;

        startKey0 = _neutral_Key0;
        startKey1 = _neutral_Key1;
        startKey2 = _neutral_Key2;
        startKeyBlend0 = _neutral_KeyBlend0;
        startKeyBlend1 = _neutral_KeyBlend1;
        startKeyBlend2 = _neutral_KeyBlend2;

        targetKey0 = startKey0;
        targetKey1 = startKey1;
        targetKey2 = startKey2;
        targetKeyBlend0 = startKeyBlend0;
        targetKeyBlend1 = startKeyBlend1;
        targetKeyBlend2 = startKeyBlend2;

        _leavesGradient = new Gradient();
        _leavesGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(startKey0, 0f),
                new GradientColorKey(startKey1, 0.1f),
                new GradientColorKey(startKey2, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );

        _vfx.SetGradient("Leaves Gradient", _leavesGradient);

        _blendGradient = new Gradient();
        _blendGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(startKeyBlend0, 0f),
                new GradientColorKey(startKeyBlend1, 0.5f),
                new GradientColorKey(startKeyBlend2, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );

        _vfx.SetGradient("Blend Gradient", _blendGradient);

        keys = _leavesGradient.colorKeys;
        keysBlend = _blendGradient.colorKeys;
    }
    void Update()
    {
        FallLeaves();

        if (_generalController.MoodChanging)
        {
            moodType = _generalController.Mood;

            if (moodType != previousMoodType)
            {
                UpdateStartColorsFromCurrentGradient(keys, keysBlend);
                previousMoodType = moodType;
            }

            UpdateGradientOverTime(keys, keysBlend, _vegetationController.GetTransitionProgress());
        }
    }

    private void UpdateGradientOverTime(GradientColorKey[] keys, GradientColorKey[] keysBlend, float progress)
    {
        if (progress > 0.3f && progress < 1f)
        {
            float t = Mathf.InverseLerp(0.3f, 1f, progress);

            if (moodType == "sad")
            {
                targetKey0 = _sad_Key0;
                targetKey1 = _sad_Key1;
                targetKey2 = _sad_Key2;

                targetKeyBlend0 = _sad_Key0;
                targetKeyBlend1 = _sad_KeyBlend1;
                targetKeyBlend2 = _sad_KeyBlend2;
            }
            else if (moodType == "neutral")
            {
                targetKey0 = _neutral_Key0;
                targetKey1 = _neutral_Key1;
                targetKey2 = _neutral_Key2;

                targetKeyBlend0 = _neutral_KeyBlend0;
                targetKeyBlend1 = _neutral_KeyBlend1;
                targetKeyBlend2 = _neutral_KeyBlend2;
            }
            else if (moodType == "stressed")
            {
                targetKey0 = _stressed_Key0;
                targetKey1 = _stressed_Key1;
                targetKey2 = _stressed_Key2;

                targetKeyBlend0 = _stressed_KeyBlend0;
                targetKeyBlend1 = _stressed_KeyBlend1;
                targetKeyBlend2 = _stressed_KeyBlend2;
            }

            BlendColorGradientTransition(keysBlend, t);
            LeavesColorGradientTransition(keys, t);
        }
        else if (progress >= 1f)
        {
            startKey0 = targetKey0;
            startKey1 = targetKey1;
            startKey2 = targetKey2;

            startKeyBlend0 = targetKeyBlend0;
            startKeyBlend1 = targetKeyBlend1;
            startKeyBlend2 = targetKeyBlend2;
        }
    }

    private void UpdateStartColorsFromCurrentGradient(GradientColorKey[] keys, GradientColorKey[] keysBlend)
    {
        startKey0 = keys[0].color;
        startKey1 = keys[1].color;
        startKey2 = keys[keys.Length - 1].color;

        startKeyBlend0 = keysBlend[0].color;
        startKeyBlend1 = keysBlend[1].color;
        startKeyBlend2 = keysBlend[keysBlend.Length - 1].color;
    }

    private void FallLeaves()
    {
        if (_vegetationController.GetTransitionProgress() >= 0.3f)
        {
            fall = false;
        }
        _vfx.SetBool("Fall", fall);
    }

    public void ChangeMood()
    {
        moodIndex = moodType == "neutral" ? 0 :
                        moodType == "sad" ? 1 :
                        moodType == "stressed" ? 2 : -1;

        _vfx.SetInt("MoodIndex", moodIndex);
    }

    private void LeavesColorGradientTransition(GradientColorKey[] keys, float t)
    {
        keys[0].color = Color.Lerp(startKey0, targetKey0, t);
        keys[1].color = Color.Lerp(startKey1, targetKey1, t);
        keys[keys.Length - 1].color = Color.Lerp(startKey2, targetKey2, t);
        _leavesGradient.colorKeys = keys;
        _vfx.SetGradient("Leaves Gradient", _leavesGradient);
    }

    private void BlendColorGradientTransition(GradientColorKey[] keysBlend, float t)
    {
        keysBlend[0].color = Color.Lerp(startKeyBlend0, targetKeyBlend0, t);
        keysBlend[1].color = Color.Lerp(startKeyBlend1, targetKeyBlend1, t);
        keysBlend[keysBlend.Length - 1].color = Color.Lerp(startKeyBlend2, targetKeyBlend2, t);
        _blendGradient.colorKeys = keysBlend;
        _vfx.SetGradient("Blend Gradient", _blendGradient);
    }
}
