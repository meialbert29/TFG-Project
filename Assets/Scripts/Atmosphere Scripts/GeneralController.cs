using NUnit.Framework;
using Assets.LSL4Unity.Scripts.Examples;
using System.Collections.Generic;
using UnityEngine;


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
    [SerializeField] private WindData globalWind;
    [SerializeField] private CloudsController _cloudsController;
    [SerializeField] private RainController _rainController;
    [SerializeField] private ExampleFloatInlet museController;
    [SerializeField] private KeyboardInputController keyboardInputController;
    [SerializeField] private WavesReader wavesReader;
    [SerializeField] private GameObject pausedUI;

    public List<VegetationController> treesList = new List<VegetationController>();   

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
    public WindData GlobalWind { get { return globalWind; } }
    public int contTrees { get { return cont; } set { cont = value; } }

    private void Awake()
    {
        treesList = new List<VegetationController>(FindObjectsByType<VegetationController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));

        //PlayerPrefs.SetInt("GameMode", 0);
        int gameMode = PlayerPrefs.GetInt("GameMode");

        Debug.Log(gameMode);
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
            keyboardInputController.KeyDown = false;

            if (!windChanging)
            {
                ChangeWindParameters();
                windChanging = true;
            }

            ApplyWind();
        }
    }

    public void CheckTreesCount()
    {
        if (cont >= treesList.Count)
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
}
