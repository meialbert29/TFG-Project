using Assets.LSL4Unity.Scripts.Examples;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WavesReader : MonoBehaviour
{
    private ExampleFloatInlet eeg_script;
    private GeneralController _generalController;

    private List<VegetationController> treesList = new List<VegetationController>();

    private string _currentWave;
    private float waveConsistencyTimer = 0f;
    private const float waveConsistencyDuration = 3f;
    private bool isWaveConsistent = false;
    private string lastWaveThatTriggeredMorph = "";

    void Start()
    {
        eeg_script = FindAnyObjectByType<ExampleFloatInlet>();
        _generalController = FindAnyObjectByType<GeneralController>();

        treesList = _generalController.TreesList;
    }

    // Update is called once per frame
    void Update()
    {
        HandleWaveConsistency();
    }

    private string GetCurrentWave()
    {
        return eeg_script.lastWaveType;
    }

    public void HandleWaveConsistency()
    {
        string newWave = GetCurrentWave();

        if (newWave == _currentWave)
        {
            waveConsistencyTimer += Time.deltaTime;

            if (waveConsistencyTimer >= waveConsistencyDuration && !isWaveConsistent)
            {
                isWaveConsistent = true;

                if (newWave != lastWaveThatTriggeredMorph)
                {
                    Debug.Log("Wave consistent and different from last morph: " + newWave);
                    lastWaveThatTriggeredMorph = newWave;

                    _generalController.MoodChanging = true;
                }
            }
        }
        else
        {
            waveConsistencyTimer = 0f;
            isWaveConsistent = false;
            _currentWave = newWave;

            foreach (var tree in treesList)
            {
                tree.LoadTargetMesh(_currentWave);
            }
        }
    }
}
