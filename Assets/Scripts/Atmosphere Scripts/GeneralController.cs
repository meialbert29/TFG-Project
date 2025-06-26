using NUnit.Framework;
using Assets.LSL4Unity.Scripts.Examples;
using System.Collections.Generic;
using UnityEngine;
using Assets.LSL4Unity.Scripts.AbstractInlets;


public class GeneralController : MonoBehaviour
{
    public struct WindData
    {
        public Vector3 direction;
        public float speed;

        public Vector4 TransformToRotateProjection()
        {
            return new Vector4(direction.x, 0f, 0f, 90);
        }
    }
    [Header("Audio Manager")]
    [SerializeField] private AudioManager audioManager;
    [Header("Scripts")]
    [SerializeField] private WindData globalWind;
    [SerializeField] private CloudsController _cloudsController;
    [SerializeField] private RainController _rainController;
    [SerializeField] private ExampleFloatInlet museController;
    [SerializeField] private LSLStreamDebugger streamDebugger;
    [SerializeField] private KeyboardInputController keyboardInputController;
    [SerializeField] private WavesReader wavesReader;
    [SerializeField] private SunController sunController;
    [SerializeField] private FogController fogController;
    
    [Header("GameObjects")]
    [SerializeField] private GameObject pausedUI;

    private List<VegetationController> treesList = new List<VegetationController>();   
    private List<VegetationController> bushesList = new List<VegetationController>();
    private List<GameObject> rocksList = new List<GameObject>();

    public int cont = 0;
    // state variables
    private string _mood = "neutral";
    private bool moodChanging = false;
    private bool windChanging = false;

    private Vector3 localDir;
    private float localSpeed;

    // getters & setters
    public string Mood { get { return _mood; } set { _mood = value; } }
    public bool MoodChanging { get { return moodChanging; } set { moodChanging = value; } }
    public List<VegetationController> TreesList { get { return treesList; } }
    public List<VegetationController> BushesList { get { return bushesList; } }
    public List<GameObject> RocksList { get { return rocksList; } }
    public WindData GlobalWind { get { return globalWind; } }
    public int contTrees { get { return cont; } set { cont = value; } }

    private int gameMode;

    private void Awake()
    {
        treesList = new List<VegetationController>(FindObjectsByType<VegetationController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));

        gameMode = PlayerPrefs.GetInt("GameMode");

        if (gameMode == 0)
        {
            // Manual mode
            keyboardInputController.gameObject.SetActive(true);

            wavesReader.gameObject.SetActive(false);
        }
        else
        {
            // Muse mode
            keyboardInputController.gameObject.SetActive(false);
            museController.StartMuseConnection();
            museController.StartMuseController();
        }

        _mood = "neutral";
    }

    private void Start()
    {
        globalWind.speed = 0.5f;
        globalWind.direction = new Vector3(0.5f, 0, 0.5f); 
    }

    // Update is called once per frame
    void Update()
    {
        if (moodChanging)
        {
            _rainController.RainChangeSettings(_mood);
            if(gameMode == 1 )
                ChangeFogAndSunlight();
            keyboardInputController.KeyDown = false;

            if (!windChanging)
            {
                ChangeWindParameters();
                windChanging = true;
            }

            ApplyWind();
        }
    }

    public void CheckVegetationCount()
    {
        if (cont >= (treesList.Count + bushesList.Count))
        {
            moodChanging = false;
            keyboardInputController.KeyDown = false;
        }
    }

    private void ApplyWind()
    {
        _rainController.UpdateWind(globalWind.speed, globalWind.direction);

        Vector4 rotateProjection = globalWind.TransformToRotateProjection();
        _cloudsController.UpdateWind(globalWind.speed, rotateProjection);

        foreach (var tree in treesList)
        {
            var leaves = tree.GetComponentInChildren<LeavesVFXController>();
            leaves.UpdateWind(localDir);
        }
    }

    public void ChangeWindParameters()
    {
        globalWind.speed = Random.Range(0.3f, 1.5f);
        globalWind.direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        localDir = Quaternion.Euler(0, Random.Range(-10f, 10f), 0) * globalWind.direction;
    }

    public void SetMood(string newMood)
    {
        if (_mood != newMood)
        {
            _mood = newMood;
            moodChanging = true;
            windChanging = false;

            switch (_mood)
            {
                case "neutral":
                    audioManager.ChangeMusicWithMixerFade(audioManager.neutralMusic);
                    audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, null);
                    fogController.DisableFog();
                    break;
                case "sad":
                    audioManager.ChangeMusicWithMixerFade(audioManager.sadMusic);
                    audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, audioManager.softRain);
                    fogController.ChangeFogSettings(true, ColorsPalette.FogColors.lightGray, 0.03f);
                    break;
                case "calm":
                    audioManager.ChangeMusicWithMixerFade(audioManager.calmMusic);
                    audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, null);
                    fogController.DisableFog();
                    break;
                case "stressed":
                    audioManager.ChangeMusicWithMixerFade(audioManager.stressMusic);
                    audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, audioManager.normalRain);
                    fogController.ChangeFogSettings(true, Color.gray, 0.07f);
                    break;
                case "anxious":
                    audioManager.ChangeMusicWithMixerFade(audioManager.anxiousMusic);
                    audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, audioManager.normalRain);
                    fogController.ChangeFogSettings(true, ColorsPalette.FogColors.darkGray, 0.10f);
                    break;
            }

            sunController.SetLightState(_mood);
        }
    }

    private void ChangeFogAndSunlight()
    {
        switch (_mood)
        {
            case "neutral":
                audioManager.ChangeMusicWithMixerFade(audioManager.neutralMusic);
                audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, null);
                fogController.DisableFog();
                break;
            case "sad":
                audioManager.ChangeMusicWithMixerFade(audioManager.sadMusic);
                audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, audioManager.softRain);
                fogController.ChangeFogSettings(true, ColorsPalette.FogColors.lightGray, 0.03f);
                break;
            case "calm":
                audioManager.ChangeMusicWithMixerFade(audioManager.calmMusic);
                audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, null);
                fogController.DisableFog();
                break;
            case "stressed":
                audioManager.ChangeMusicWithMixerFade(audioManager.stressMusic);
                audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, audioManager.normalRain);
                fogController.ChangeFogSettings(true, Color.gray, 0.07f);
                break;
            case "anxious":
                audioManager.ChangeMusicWithMixerFade(audioManager.anxiousMusic);
                audioManager.ChangeSFXWithMixerFade(audioManager.neutralWind, audioManager.normalRain);
                fogController.ChangeFogSettings(true, ColorsPalette.FogColors.darkGray, 0.10f);
                break;
        }

        sunController.SetLightState(_mood);
    }
}
