using System;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.ParticleSystem;

public class VFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private Gradient leavesGradient;
    [SerializeField] private Gradient blendGradient;
    [SerializeField] public bool fall;
    [SerializeField] private VegetationBehaviour vb;
    [SerializeField] private int moodIndex;
    

    private Color startKey0, startKey1, startKey2;
    private Color targetKey0, targetKey1, targetKey2;
    private Color startKeyBlend0, startKeyBlend1, startKeyBlend2;
    private Color targetKeyBlend0, targetKeyBlend1, targetKeyBlend2;

    private Color default0 = new Color(0.286961f, 0.4150943f, 0.1240061f, 1f);
    private Color default1 = new Color(0.2944696f, 0.3836477f, 0.1315018f, 1f);
    private Color default2 = new Color(0.08877103f, 0.2f, 0.06666668f, 1f);

    private Color stressed0 = new Color(0.1509434f, 0.1509434f, 0.1509434f, 1f);
    private Color stressed1 = new Color(0.06607719f, 0.06925166f, 0.08176088f, 1f);
    private Color stressed2 = new Color(0.01014595f, 0.01096753f, 0.01886791f, 1f);
    private Color stressedBlend0 = new Color(0.01161224f, 0.01298303f, 0.02121901f, 1f);
    private Color stressedBlend1 = new Color(0.08176088f, 0.08176088f, 0.08176088f, 1f);
    private Color stressedBlend2 = new Color(0.1301365f, 0.1301365f, 0.1328684f);

    private Color sad0 = new Color(0.1635219f, 0.02678018f, 0, 1f);
    private Color sad1 = new Color(0.2578616f, 0.04968183f, 0f, 1f);
    private Color sad2 = new Color(0.4784314f, 0.2680945f, 0.08235293f, 1f);
    private Color sadBlend1 = new Color(0.4901961f, 0.2157505f, 0f, 1f);
    private Color sadBlend2 = new Color(0.4823529f, 0.3290052f, 0.1333334f);

    private Color blend0 = new Color(0.04859377f, 0.1698113f, 0.05436604f, 1f);
    private Color blend1 = new Color(0.1579446f, 0.4150943f, 0.172345f, 1f);
    private Color blend2 = new Color(0.2587516f, 0.5597484f, 0.3027174f);

    private string moodType;
    private string previousMoodType;

    GradientColorKey[] keys;
    GradientColorKey[] keysBlend;

    void Start()
    {
        vb = transform.GetComponent<VegetationBehaviour>();

        moodType = vb.mood;
        previousMoodType = moodType;
        moodIndex = 0;

        startKey0 = default0;
        startKey1 = default1;
        startKey2 = default2;
        startKeyBlend0 = blend0;
        startKeyBlend1 = blend1;
        startKeyBlend2 = blend2;

        targetKey0 = startKey0;
        targetKey1 = startKey1;
        targetKey2 = startKey2;
        targetKeyBlend0 = startKeyBlend0;
        targetKeyBlend1 = startKeyBlend1;
        targetKeyBlend2 = startKeyBlend2;

        leavesGradient = new Gradient();
        leavesGradient.SetKeys(
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

        vfx.SetGradient("Leaves Gradient", leavesGradient);

        blendGradient = new Gradient();
        blendGradient.SetKeys(
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

        vfx.SetGradient("Blend Gradient", blendGradient);

        keys = leavesGradient.colorKeys;
        keysBlend = blendGradient.colorKeys;
    }

    void Update()
    {
        fallLeaves();

        if (vb.getMorphingState())
        {
            moodType = vb.mood;

            if (moodType != previousMoodType)
            {
                UpdateStartColorsFromCurrentGradient(keys, keysBlend);
                previousMoodType = moodType;
            }

            updateGradientOverTime(keys, keysBlend, vb.getTransitionProgress());
        }
    }

    private void updateGradientOverTime(GradientColorKey[] keys, GradientColorKey[] keysBlend, float progress)
    {
        if (progress > 0.3f && progress < 1f)
        {
            float t = Mathf.InverseLerp(0.3f, 1f, progress);

            if (moodType == "sad")
            {
                targetKey0 = sad0;
                targetKey1 = sad1;
                targetKey2 = sad2;

                targetKeyBlend0 = sad0;
                targetKeyBlend1 = sadBlend1;
                targetKeyBlend2 = sadBlend2;
            }
            else if (moodType == "neutral")
            {
                targetKey0 = default0;
                targetKey1 = default1;
                targetKey2 = default2;

                targetKeyBlend0 = blend0;
                targetKeyBlend1 = blend1;
                targetKeyBlend2 = blend2;
            }
            else if (moodType == "stressed")
            {
                targetKey0 = stressed0;
                targetKey1 = stressed1;
                targetKey2 = stressed2;

                targetKeyBlend0 = stressedBlend0;
                targetKeyBlend1 = stressedBlend1;
                targetKeyBlend2 = stressedBlend2;
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

    private void fallLeaves()
    {
        vfx.SetBool("Fall", fall);
    }

    public void changeMood()
    {
        moodIndex = moodType == "neutral" ? 0 :
                        moodType == "sad" ? 1 :
                        moodType == "stressed" ? 2 : -1;

        vfx.SetInt("MoodIndex", moodIndex);
    }

    private void LeavesColorGradientTransition(GradientColorKey[] keys, float t)
    {
        keys[0].color = Color.Lerp(startKey0, targetKey0, t);
        keys[1].color = Color.Lerp(startKey1, targetKey1, t);
        keys[keys.Length - 1].color = Color.Lerp(startKey2, targetKey2, t);
        leavesGradient.colorKeys = keys;
        vfx.SetGradient("Leaves Gradient", leavesGradient);
    }

    private void BlendColorGradientTransition(GradientColorKey[] keysBlend, float t)
    {
        keysBlend[0].color = Color.Lerp(startKeyBlend0, targetKeyBlend0, t);
        keysBlend[1].color = Color.Lerp(startKeyBlend1, targetKeyBlend1, t);
        keysBlend[keysBlend.Length - 1].color = Color.Lerp(startKeyBlend2, targetKeyBlend2, t);
        blendGradient.colorKeys = keysBlend;
        vfx.SetGradient("Blend Gradient", blendGradient);
    }
}
