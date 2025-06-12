using Assets.LSL4Unity.Scripts.Examples;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.VFX;

public class VegetationController : MonoBehaviour
{
    private Renderer vegetationRenderer;
    [SerializeField] private ExampleFloatInlet eeg_script;
    [SerializeField] private VFXController _vfxController;
    [SerializeField] private LeavesController _leavesController;
    [SerializeField] private GeneralController _generalController;
    [SerializeField] private WavesReader _wavesReader;

    // wave variables
    private string _currentWave;
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
    
    float currentLeavesVisibility;
    float latestLeavesVisibility;

    // progress
    private float _transitionProgress = 0f;
    private float _transitionDuration = 5f;

    // bools
    private bool _isMorphing = false;
    private bool alreadyChanged = false;
    private bool firstTime = true;
    private bool isTransitioning = false;

    // others
    public List<VegetationController> neighbourVegetation;
    private string vegetationType;


    public bool IsMorphing { get { return _isMorphing; } set { _isMorphing = value; } }

    void Awake()
    {

        if(eeg_script == null || _generalController == null)
        {
            Debug.Log("Error in Vegetation Behaviour");
            return;
        }

        //vfx = transform.GetComponent<VFXController>();
        if (_vfxController == null) Debug.Log("VFX Graph not found");

        //leavesController = transform.GetComponent<LeavesController>();
        if (_leavesController == null) Debug.Log("leaves Controller not found");
        

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

        if (startMesh == null)
        {
            Debug.LogError("Start mesh were not loaded correctly. Please check the path.");
            return;
        }

        morphedMesh = new Mesh();
        meshFilter.mesh = startMesh;
        morphedMesh.vertices = startMesh.vertices;
        morphedMesh.triangles = startMesh.triangles;
        morphedMesh.normals = startMesh.normals;
        morphedMesh.uv = startMesh.uv;
    }

    void Update()
    {
        if (_generalController.MoodChanging)
        {
            _vfxController.Fall = true;
            LoadTargetMesh(_wavesReader.CurrentWave);
            LoadTargetMeshByMood(_generalController.Mood);
            MorphingProcess();
        }
    }

    //private void HandleWaveConsistency()
    //{
    //    string newWave = GetCurrentWave();

    //    if (newWave == _currentWave)
    //    {
    //        waveConsistencyTimer += Time.deltaTime;

    //        if (waveConsistencyTimer >= waveConsistencyDuration && !isWaveConsistent)
    //        {
    //            isWaveConsistent = true;

    //            if (newWave != lastWaveThatTriggeredMorph)
    //            {
    //                Debug.Log("Wave consistent and different from last morph: " + newWave);
    //                lastWaveThatTriggeredMorph = newWave;
    //                _generalController.MoodChanging = true;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        waveConsistencyTimer = 0f;
    //        isWaveConsistent = false;
    //        _currentWave = newWave;
    //        LoadTargetMesh(_currentWave);
    //    }
    //}


    public void LoadTargetMesh(string wave)
    {
        switch (wave)
        {
            case "Delta":
                targetMesh = Resources.Load<Mesh>("Models/Trunks/SadTrunk");
                _generalController.Mood = "sad";
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
                _generalController.Mood = "stressed";
                break;
        }
    }

    public void LoadTargetMeshByMood(string mood)
    {
        string path = "Models/Trunks/";
        string meshName = "";
        switch (mood)
        {
            case "sad":
                meshName = "SadTrunk";
                break;
            case "neutral":
                meshName = "Trunk";
                break;
            case "stressed":
                meshName = "StressedTrunk";
                break;
            case "calm":
                meshName = "Trunk";
                break;
            case "anxious":
                meshName = "StressedTrunk";
                break;

        }
        targetMesh = Resources.Load<Mesh>($"{path}{meshName}");
    }

    public void MorphingProcess()
    {
        _transitionProgress += Time.deltaTime / _transitionDuration;

        if (_transitionProgress >= 0.3f)
        {
            _vfxController.ChangeMood();
        }

        if (_transitionProgress >= 1f)
        {
            _transitionProgress = 0f;
            startMesh = targetMesh;
            //_isMorphing = false;
            //_generalController.MoodChanging = false;

            _generalController.cont++;
            _vfxController.Fall = false;
            _generalController.CheckTreesCount();
            
            _leavesController.UpdateLeavesStartColors();
            Debug.Log("Transition finished");
        }

        MorphMeshes(startMesh, targetMesh, _transitionProgress);
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
            path = "Models/Trunks/";

        targetMesh = Resources.Load<Mesh>($"{path}{meshName}");
        _transitionProgress = 0f;
    }

    private void StartMorphing()
    {
        Debug.Log($"Onda consistente durante {waveConsistencyDuration} segundos. Activando morphing.");
        _isMorphing = true;
    }

    //public void MorphNeighbours(VegetationController vegetationMorphing)
    //{
    //    alreadyChanged = false;

    //    if (latestState != actualState)
    //    {
    //        _isMorphing = true;
    //        latestState = actualState;
    //    }

    //    if (!alreadyChanged)
    //        SetTargetMesh(vegetationMorphing.actualMesh, vegetationMorphing.actualState);

    //    if (neighbourVegetation.Count > 0)
    //    {
    //        StartCoroutine(OverlayMorphing());
    //    }
    //}

    //private IEnumerator OverlayMorphing()
    //{
    //    yield return new WaitForSeconds(3f);

    //    foreach (VegetationController neighbour in neighbourVegetation)
    //    {
    //        if (!neighbour.alreadyChanged)
    //            neighbour.MorphNeighbours(neighbour);
    //    }
    //}

    public float GetTransitionProgress()
    {
        return _transitionProgress;
    }

    public bool getMorphingState()
    {
        return _isMorphing;
    }
}
