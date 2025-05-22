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

    // state variables
    private string _mood = "neutral";
    private bool moodChanging = false;

    // getters & setters
    public string Mood { get { return _mood; } set { _mood = value; } }
    public bool MoodChanging { get { return moodChanging; } set { moodChanging = value; } }
    public List<VegetationController> TreesList { get { return treesList;} }

    public int cont = 0;

    private void Awake()
    {
        treesList = new List<VegetationController>(FindObjectsByType<VegetationController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        _cloudsController = FindAnyObjectByType<CloudsController>();
        _rainController = FindAnyObjectByType<RainController>();
        eeg_script = FindAnyObjectByType<ExampleFloatInlet>();

        _mood = "neutral";
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

        if (keyDown)
        {
            cont = 0;
            moodChanging = true; 
        }
    }

    public void checkCont()
    {
        if (cont >= treesList.Count)
            moodChanging = false;
    }
}
