using NUnit.Framework;
using Assets.LSL4Unity.Scripts.Examples;
using System.Collections.Generic;
using UnityEngine;

public class GeneralController : MonoBehaviour
{
    private CloudsController _cloudsController;
    private RainController _rainController;
    public List<VegetationController> treesList = new List<VegetationController>();
    private ExampleFloatInlet eeg_script;

    // global wind variables
    float windSpeed;
    Vector3 windDirection;

    // state variables
    private string _mood = "neutral";
    private bool moodChanging = false;

    // getters & setters
    public string Mood { get { return _mood; } set { _mood = value; } }
    public bool MoodChanging { get { return moodChanging; } set { moodChanging = value; } }
    public List<VegetationController> TreesList { get { return treesList;} }

    public int cont = 0;

    public GameObject pausedUI;

    private float randomSpeed;
    private Vector3 randomDirection;

    // getters & setters
    // global wind variables
    public float WindSpeed { get; private set; }
    public Vector3 WindDirection { get; private set; }

    private void Awake()
    {
        treesList = new List<VegetationController>(FindObjectsByType<VegetationController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        _cloudsController = FindAnyObjectByType<CloudsController>();
        _rainController = FindAnyObjectByType<RainController>();
        //eeg_script = FindAnyObjectByType<ExampleFloatInlet>();

        _mood = "neutral";
    }

    private void Start()
    {
        windSpeed = 0.5f;
        windDirection = new Vector3(0.5f, 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    // check if any key was pulsed
    private void HandleInput()
    {
        bool keyDown = false;

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _mood = "sad";
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            _mood = "stressed";
            
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            _mood = "neutral";
            keyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            _mood = "calm";
            keyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            _mood = "anxious";
            keyDown = true;
        }

        if (keyDown) // fix to not compare if any key's down
        {
            _rainController.RainChangeSettings(_mood);
            cont = 0;
            moodChanging = true;

            windSpeed = Random.Range(0.3f, 1.5f);
            windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

            ApplyWind();
        }
    }

    public void CheckTreesCount()
    {
        if (cont >= treesList.Count)
            moodChanging = false;
    }

    private void ApplyWind()
    {
        _rainController.UpdateWind(windSpeed, windDirection);
        _cloudsController.UpdateWind(windSpeed, windDirection);

        foreach (var tree in treesList)
        {
            var leaves = tree.GetComponentInChildren<LeavesVFXController>();
            if (leaves != null)
            {
                leaves.SetWind(windDirection, windSpeed);
            }
        }
    }
}
