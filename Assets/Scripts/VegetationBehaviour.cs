using Assets.LSL4Unity.Scripts.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VegetationBehaviour : MonoBehaviour
{
    private Renderer vegetationRenderer;
    private ExampleFloatInlet eeg_script;
    private LeavesController leavesController;

    // wave variables
    private string currentWave;
    private float waveConsistencyTimer = 0f;
    private const float waveConsistencyDuration = 3f;
    private bool isWaveConsistent = false;
    private string lastWaveThatTriggeredMorph = "";

    // meshes
    private string actualState;
    private string latestState;
    private string actualMesh;
    private Transform trunkMesh;
    private MeshFilter meshFilter;
    private Mesh startMesh;
    private Mesh targetMesh;
    private Mesh morphedMesh;

    // materials
    private Material trunkMaterial;

    // leaves
    private VFXController vfx;
    float currentLeavesVisibility;
    float latestLeavesVisibility;

    // progress
    private float transitionProgress = 0f;
    private float transitionDuration = 3f;

    // bools
    private bool isMorphing = false;
    private bool alreadyChanged = false;
    private bool firstTime = true;
    private bool isTransitioning = false;

    // others
    public List<VegetationBehaviour> neighbourVegetation;
    private string vegetationType;

    public string mood;

    void Start()
    {
        // find objects
        eeg_script = FindAnyObjectByType<ExampleFloatInlet>();
        if (eeg_script == null) Debug.Log("EEG Script not found");

        vfx = transform.GetComponent<VFXController>();
        if (vfx == null) Debug.Log("VFX Graph not found");

        leavesController = transform.GetComponent<LeavesController>();
        if (leavesController == null) Debug.Log("leaves Controller not found");

        // get mesh object
        trunkMesh = transform.GetChild(1);
        meshFilter = trunkMesh.GetComponent<MeshFilter>();

        // get object tag
        vegetationType = gameObject.tag;

        // check if the object is in the Vegetation Layer
        if (gameObject.layer == LayerMask.NameToLayer("Vegetation"))
        {
            startMesh = Resources.Load<Mesh>("Models/Trunks/Trunk");
            targetMesh = Resources.Load<Mesh>("Models/Trunks/SadTrunk");
        }

        if (startMesh == null || targetMesh == null)
        {
            Debug.LogError("Meshes were not loaded correctly. Please check the path.");
            return;
        }

        morphedMesh = new Mesh();
        meshFilter.mesh = startMesh;
        morphedMesh.vertices = startMesh.vertices;
        morphedMesh.triangles = startMesh.triangles;
        morphedMesh.normals = startMesh.normals;
        morphedMesh.uv = startMesh.uv;

        mood = "neutral";
    }

    void Update()
    {
        HandleWaveConsistency();
        HandleInput();
        if (isMorphing)
        {
            vfx.fall = true;
            MorphingProcess();
        }
    }

    private void HandleWaveConsistency()
    {
        string newWave = GetCurrentWave();

        if (newWave == currentWave)
        {
            waveConsistencyTimer += Time.deltaTime;

            if (waveConsistencyTimer >= waveConsistencyDuration && !isWaveConsistent)
            {
                isWaveConsistent = true;

                if (newWave != lastWaveThatTriggeredMorph) // ✅ Añadido
                {
                    Debug.Log("Wave consistent and different from last morph: " + newWave);
                    lastWaveThatTriggeredMorph = newWave; // ✅ Añadido
                    isMorphing = true;
                }
                //else
                //{
                //    Debug.Log("Wave is consistent but same as last one, skipping morph."); // ✅ Añadido
                //}
            }
        }
        else
        {
            waveConsistencyTimer = 0f;
            isWaveConsistent = false;
            currentWave = newWave;
            MorphingByWave(currentWave);
        }
    }

    private string GetCurrentWave()
    {
        return eeg_script.lastWaveType;
    }

    private void MorphingByWave(string wave)
    {
        switch (wave)
        {
            case "Delta":
                targetMesh = Resources.Load<Mesh>("Models/Trunks/SadTrunk");
                mood = "sad";
                break;
            case "Theta":
                targetMesh = Resources.Load<Mesh>("Models/Sad Tree/SadTree");
                break;
            case "Alpha":
                targetMesh = Resources.Load<Mesh>("Models/Tree Trunk/SadTree");
                break;
            case "Beta":
                targetMesh = Resources.Load<Mesh>("Models/Meshes/Tree Trunk/SadTree");
                break;
            case "Gamma":
                targetMesh = Resources.Load<Mesh>("Models/Trunks/StressedTrunk");
                mood = "stressed";
                break;
        }
    }

    public void MorphingProcess()
    {
        transitionProgress += Time.deltaTime / transitionDuration;

        if (transitionProgress >= 0.3f)
        {
            vfx.changeMood();
        }

        if (transitionProgress > 1f)
        {
            transitionProgress = 0f;
            startMesh = targetMesh;
            isMorphing = false;
            vfx.fall = false;
            leavesController.updateLeavesStartColors();
            Debug.Log("Transition finished");
        }

        MorphMeshes(startMesh, targetMesh, transitionProgress);
    }

    void MorphMeshes(Mesh fromMesh, Mesh toMesh, float t)
    {
        if (fromMesh.vertexCount != toMesh.vertexCount)
        {
            Debug.LogError("Meshes do not have the same number of vertices.");
            return;
        }

        Vector3[] fromVertices = fromMesh.vertices;
        Vector3[] toVertices = toMesh.vertices;
        Vector3[] morphedVertices = new Vector3[fromVertices.Length];

        for (int i = 0; i < fromVertices.Length; i++)
        {
            morphedVertices[i] = Vector3.Lerp(fromVertices[i], toVertices[i], t);
        }

        morphedMesh.vertices = morphedVertices;
        morphedMesh.triangles = fromMesh.triangles;
        morphedMesh.RecalculateNormals();
        meshFilter.mesh = morphedMesh;
    }

    public void SetTargetMesh(string meshName, string state)
    {
        actualState = state;

        startMesh = meshFilter.mesh;
        string path = "";

        if (gameObject.layer == LayerMask.NameToLayer("Vegetation"))
            path = "Models/";

        targetMesh = Resources.Load<Mesh>($"{path}{meshName}");
        transitionProgress = 0f;
    }

    private void StartMorphing()
    {
        Debug.Log($"Onda consistente durante {waveConsistencyDuration} segundos. Activando morphing.");
        isMorphing = true;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            mood = "sad";
            isMorphing = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            mood = "stressed";
            isMorphing = true;
            targetMesh = Resources.Load<Mesh>("Models/Trunks/StressedTrunk");
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            mood = "neutral";
            isMorphing = true;
            targetMesh = Resources.Load<Mesh>("Models/Trunks/Trunk");
        }
    }

    public void MorphNeighbours(VegetationBehaviour vegetationMorphing)
    {
        alreadyChanged = false;

        if (latestState != actualState)
        {
            isMorphing = true;
            latestState = actualState;
        }

        if (!alreadyChanged)
            SetTargetMesh(vegetationMorphing.actualMesh, vegetationMorphing.actualState);

        if (neighbourVegetation.Count > 0)
        {
            StartCoroutine(OverlayMorphing());
        }
    }

    private IEnumerator OverlayMorphing()
    {
        yield return new WaitForSeconds(3f);

        foreach (VegetationBehaviour neighbour in neighbourVegetation)
        {
            if (!neighbour.alreadyChanged)
                neighbour.MorphNeighbours(neighbour);
        }
    }

    public float getTransitionProgress()
    {
        return transitionProgress;
    }
}
