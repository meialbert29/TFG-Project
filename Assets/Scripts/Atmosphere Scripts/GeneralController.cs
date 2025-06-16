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
    private WindData globalWind;
    private CloudsController _cloudsController;
    private RainController _rainController;
    public List<VegetationController> treesList = new List<VegetationController>();
    [SerializeField] private ExampleFloatInlet museController;
    [SerializeField] private KeyboardInputController keyboardInputController;
    public List<VegetationController> TreesList { get { return treesList; } }
    public GameObject pausedUI;

    public int cont = 0;
    // state variables
    private string _mood = "neutral";
    private bool moodChanging = false;

    // getters & setters
    public string Mood { get { return _mood; } set { _mood = value; } }
    public bool MoodChanging { get { return moodChanging; } set { moodChanging = value; } }
    public WindData GlobalWind { get { return globalWind; } }

    private void Awake()
    {
        treesList = new List<VegetationController>(FindObjectsByType<VegetationController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        _cloudsController = FindAnyObjectByType<CloudsController>();
        _rainController = FindAnyObjectByType<RainController>();

        int gameMode = PlayerPrefs.GetInt("GameMode", 0);

        if (gameMode == 0)
        {
            // Manual mode
            keyboardInputController.gameObject.SetActive(true);

            museController.gameObject.SetActive(false);
        }
        else
        {
            // Muse mode
            keyboardInputController.gameObject.SetActive(false);
            museController.gameObject.SetActive(true);
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
        if (keyboardInputController != null && keyboardInputController.KeyDown)
        {
            _rainController.RainChangeSettings(_mood);
            cont = 0;
            moodChanging = true;

            ChangeWindParameters();
        }
    }
    public void CheckTreesCount()
    {
        if (cont >= treesList.Count)
            moodChanging = false;
    }

    private void ApplyWind()
    {
        _rainController.UpdateWind(globalWind.speed, globalWind.direction);

        Vector4 rotateProjection = globalWind.TransformToRotateProjection();
        _cloudsController.UpdateWind(globalWind.speed, rotateProjection);

        foreach (var tree in treesList)
        {
            var leaves = tree.GetComponentInChildren<LeavesVFXController>();
            if (leaves != null)
            {
                Vector3 localDir = Quaternion.Euler(0, Random.Range(-10f, 10f), 0) * globalWind.direction;
                float localSpeed = globalWind.speed * Random.Range(0.9f, 1.1f);

                leaves.UpdateWind(localDir, localSpeed);
            }
        }
    }

    public void ChangeWindParameters()
    {
        globalWind.speed = Random.Range(0.3f, 1.5f);
        globalWind.direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        ApplyWind();
    }

    
}
