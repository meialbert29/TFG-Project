using UnityEngine;

public static class ColorsPalette
{
    public static class LeavesColors
    {
        public static readonly Color sad_TopColor = new Color(0.7735849f, 0.4542985f, 0.2262369f, 1f);
        public static readonly Color sad_BottomColor = new Color(0.8805031f, 0.6550949f, 0.3405719f, 1f);
        public static readonly Color sad_BlendColor = new Color(0.8805031f, 0.6550949f, 0.3405719f, 1f);

        public static readonly Color neutral_TopColor = new Color(0.5644949f, 0.8930817f, 0.582952f, 1f);
        public static readonly Color neutral_BottomColor = new Color(0.5430757f, 0.7106918f, 0.2346623f, 1f);
        public static readonly Color neutral_BlendColor = new Color(1f, 1f, 1f, 1f);

        public static readonly Color calm_TopColor = new Color(0.9150943f, 0.908694f, 0.7294856f, 1f);
        public static readonly Color calm_BottomColor = new Color(0.9339623f, 0.8929893f, 0.5418743f, 1f);
        public static readonly Color calm_BlendColor = new Color(0f, 1f, 0.3122611f, 1f);

        public static readonly Color stressed_TopColor = new Color(0.06607719f, 0.06925166f, 0.08176088f, 1f);
        public static readonly Color stressed_BottomColor = new Color(0.1509434f, 0.1509434f, 0.1509434f, 1f);
        public static readonly Color stressed_BlendColor = new Color(0.1509434f, 0.1509434f, 0.1509434f, 1f);
        
        public static readonly Color anxious_TopColor = new Color(0.0754717f, 0.03239587f, 0.03471738f, 1f);
        public static readonly Color anxious_BottomColor = new Color(0.1301365f, 0.1301365f, 0.1328684f, 1f);
        public static readonly Color anxious_BlendColor = new Color(0.1301365f, 0.1301365f, 0.1328684f, 1f);
    }

    public static class TrunkColors
    {
        public static readonly Color trunk_AnxiousColor = new Color(0f, 0f, 0f, 1f);
        public static readonly Color trunk_NeutralColor = new Color(0.8396226f, 0.7110994f, 0.5425863f, 1f);
        public static readonly Color trunk_SadColor = new Color(0.3396226f, 0.2771448f, 0.2771448f, 1f);
        public static readonly Color trunk_StressedColor = new Color(0.3207547f, 0.3118332f, 0.2920078f, 1f);
        public static readonly Color trunk_CalmColor = new Color(1f, 0.9037364f, 0.7783019f, 1f);
    }

    public static class LeavesVFXColors
    {
        // neutral state
        public static readonly Color neutral_Key0 = new Color(0.286961f, 0.4150943f, 0.1240061f, 1f);
        public static readonly Color neutral_Key1 = new Color(0.2944696f, 0.3836477f, 0.1315018f, 1f);
        public static readonly Color neutral_Key2 = new Color(0.08877103f, 0.2f, 0.06666668f, 1f);

        public static readonly Color neutral_KeyBlend0 = new Color(0.04859377f, 0.1698113f, 0.05436604f, 1f);
        public static readonly Color neutral_KeyBlend1 = new Color(0.1579446f, 0.4150943f, 0.172345f, 1f);
        public static readonly Color neutral_KeyBlend2 = new Color(0.2587516f, 0.5597484f, 0.3027174f);

        // stressed state
        public static readonly Color stressed_Key0 = new Color(0.1509434f, 0.1509434f, 0.1509434f, 1f);
        public static readonly Color stressed_Key1 = new Color(0.06607719f, 0.06925166f, 0.08176088f, 1f);
        public static readonly Color stressed_Key2 = new Color(0.01014595f, 0.01096753f, 0.01886791f, 1f);
        
        public static readonly Color stressed_KeyBlend0 = new Color(0.01161224f, 0.01298303f, 0.02121901f, 1f);
        public static readonly Color stressed_KeyBlend1 = new Color(0.08176088f, 0.08176088f, 0.08176088f, 1f);
        public static readonly Color stressed_KeyBlend2 = new Color(0.1301365f, 0.1301365f, 0.1328684f, 1f);

        // sad state
        public static readonly Color sad_Key0 = new Color(0.1635219f, 0.02678018f, 0, 1f);
        public static readonly Color sad_Key1 = new Color(0.2578616f, 0.04968183f, 0f, 1f);
        public static readonly Color sad_Key2 = new Color(0.4784314f, 0.2680945f, 0.08235293f, 1f);

        public static readonly Color sad_KeyBlend1 = new Color(0.4901961f, 0.2157505f, 0f, 1f);
        public static readonly Color sad_KeyBlend2 = new Color(0.4823529f, 0.3290052f, 0.1333334f, 1f);

        // calm state
        public static readonly Color calm_Key0 = new Color(0.7058824f, 0.6392202f, 0.2117647f, 1f);
        public static readonly Color calm_Key1 = new Color(0.4501218f, 0.5843138f, 0.2196078f, 1f);
        public static readonly Color calm_Key2 = new Color(0.1700774f, 0.02f, 0.06666668f, 1f);

        public static readonly Color calm_KeyBlend0 = new Color(0.2050788f, 0.09084173f, 0.08650047f, 1f);
        public static readonly Color calm_KeyBlend1 = new Color(0.3662527f, 0.4125427f, 0.1714412f, 1f);
        public static readonly Color calm_KeyBlend2 = new Color(0.5840786f, 0.6104957f, 0.2158605f, 1f);

        // anxious state
        public static readonly Color anxious_Key0 = new Color(0.0754717f, 0.03239587f, 0.03471738f, 1f);
        public static readonly Color anxious_Key1 = new Color(0.0471698f, 0.01357244f, 0.01662675f, 1f);
        public static readonly Color anxious_Key2 = new Color(0.01014595f, 0.01096753f, 0.01886791f, 1f);
        
        public static readonly Color anxious_KeyBlend0 = new Color(0.02028856f, 0.01161224f, 0.01850022f, 1f);
        public static readonly Color anxious_KeyBlend1 = new Color(0.0471698f, 0.0471698f, 0.0471698f, 1f);
        public static readonly Color anxious_KeyBlend2 = new Color(0.1301365f, 0.1301365f, 0.1328684f, 1f);
    }

    public static class CloudsColors
    {
        public static readonly Color sadValley = new Color(0.5847869f, 0.7623752f, 0.8050313f, 1f);
        public static readonly Color sadPeak = new Color(0.3602704f, 0.4823402f, 0.553459f, 1f);

        public static readonly Color stressedValley = new Color(0.3679245f, 0.3679245f, 0.3679245f, 1f);
        public static readonly Color stressedPeak = new Color(0.4909666f, 0.5261293f, 0.5566037f, 1f);

        public static readonly Color anxiousValley = new Color(0.1834727f, 0.2329639f, 0.2924528f, 1f);
        public static readonly Color anxiousPeak = new Color(0.04846029f, 0.134976f, 0.3113208f, 1f);
    }

    public static class LogosColors
    {
        public static readonly Color sunMorning = new Color(1f, 0.915f, 0.3679244f, 0.7215686f);
        public static readonly Color sunEvening = new Color(1f, 0.8701827f, 0.5251572f, 0.7215686f);
    }

    public static class ButtonsColors
    {
        // main menu buttons
        public static readonly Color normalColor = new Color(0.5058824f, 0.2611249f, 0.2611249f, 1f);
        public static readonly Color hoverColor = Color.white;
    }

    public static class LotusColors
    {
        public static readonly Color transparent = new Color(0f, 0f, 0f, 0f);
    }

    public static class FogColors
    {
        public static readonly Color darkGray = new Color(0.3584906f, 0.3584906f, 0.3584906f, 1f);
        public static readonly Color lightGray = new Color(0.4842767f, 0.4842767f, 0.4842767f, 1f);
    }

    public static class SunColors
    {
        public static readonly Color neutral = new Color(0.6918238f, 0.6847792f, 0.6548395f, 0.5843138f);
        public static readonly Color sad = new Color(0.3300502f, 0.4164391f, 0.7044024f, 0.5f);
        public static readonly Color calm = new Color(0.2819204f, 0.5220125f, 0.2084766f, 0.5f);
        public static readonly Color stress = new Color(0.4923722f, 0.3250464f, 0.5974842f, 0.5f);
        public static readonly Color anxiety = new Color(0f, 0.17541f, 0.3522012f, 0.5f);
    }
}
