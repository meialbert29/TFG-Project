using Assets.LSL4Unity.Scripts.Examples;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WavesReader : MonoBehaviour
{
    private ExampleFloatInlet eeg_script;
    private GeneralController _generalController;

    private string _currentWave;
    private float waveConsistencyTimer = 0f;
    private const float waveConsistencyDuration = 5f;
    private bool isWaveConsistent = false;
    private string lastWaveThatTriggeredMorph = "";

    public string CurrentWave { get { return _currentWave; } }
    public bool IsWaveConsistent { get { return isWaveConsistent; } }

    void Start()
    {
        eeg_script = FindAnyObjectByType<ExampleFloatInlet>();
        _generalController = FindAnyObjectByType<GeneralController>();
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
                    _generalController.ChangeWindParameters();

                    if(newWave == "Delta" || newWave == "Beta" || newWave == "Gamma")
                    {
                        foreach (var tree in _generalController.TreesList)
                        {
                            tree.GetComponentInChildren<LeavesVFXController>().Fall = true;
                        }
                        foreach (var bush in _generalController.BushesList)
                        {
                            bush.GetComponentInChildren<LeavesVFXController>().Fall = true;
                        }
                    }
                }
            }
        }
        else
        {
            waveConsistencyTimer = 0f;
            isWaveConsistent = false;
            _currentWave = newWave;

            foreach (var tree in _generalController.TreesList)
            {
                tree.LoadTargetMesh(_currentWave);
            }
            foreach (var bush in _generalController.BushesList)
            {
                bush.LoadTargetMesh(_currentWave);
            }
        }
    }

}
