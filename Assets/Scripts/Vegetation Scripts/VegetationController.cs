using Assets.LSL4Unity.Scripts.Examples;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.VFX;

public class VegetationController : MonoBehaviour
{
    private Renderer vegetationRenderer;
    [SerializeField] private ExampleFloatInlet _museController;
    [SerializeField] private LeavesVFXController _vfxController;
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

        if(_museController == null || _generalController == null)
        {
            Debug.Log(_museController);
            Debug.Log(_generalController);
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
            if(gameObject.tag == "Tree")
            {
                startMesh = Resources.Load<Mesh>("Models/Tree/Trunks/NormalTrunk");
                targetMesh = Resources.Load<Mesh>("Models/Tree/Trunks/SadTrunk");
            }
            else if(gameObject.tag == "Bush")
            {
                startMesh = Resources.Load<Mesh>("Models/Bush/Trunks/NormalTrunk");
                targetMesh = Resources.Load<Mesh>("Models/Bush/Trunks/SadTrunk");
            }
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
            LoadTargetMesh(_wavesReader.CurrentWave);
            LoadTargetMeshByMood(_generalController.Mood);
            MorphingProcess();
        }
    }

    public void LoadTargetMesh(string wave)
    {
        //switch (wave)
        //{
        //    case "Delta":
        //        targetMesh = Resources.Load<Mesh>("Models/Tree/Trunks/SadTrunk");
        //        _generalController.Mood = "sad";
        //        break;
        //    case "Theta":
        //        targetMesh = Resources.Load<Mesh>("Models/Tree/Trunks/SadTrunk");
        //        _generalController.Mood = "sad";
        //        break;
        //    case "Alpha":
        //        targetMesh = Resources.Load<Mesh>("Models/Tree/Trunks/Trunk");
        //        _generalController.Mood = "calm";
        //        break;
        //    case "Beta":
        //        targetMesh = Resources.Load<Mesh>("Models/Tree/Trunks/StressedTrunk");
        //        _generalController.Mood = "stressed";
        //        break;
        //    case "Gamma":
        //        targetMesh = Resources.Load<Mesh>("Models/Tree/Trunks/AnxiousTrunk");
        //        _generalController.Mood = "anxious";
        //        break;
        //}

        string path = "";
        string meshName = "";

        switch (wave)
        {
            case "Delta":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "NormalTrunk";
                //_generalController.Mood = "neutral";
                break;

            case "Theta":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "SadTrunk";
                //_generalController.Mood = "sad";
                break;

            case "Alpha":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "NormalTrunk";
               // _generalController.Mood = "calm";
                break;

            case "Beta":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "StressedTrunk";
                //_generalController.Mood = "stressed";
                break;

            case "Gamma":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "StressedTrunk";
                //_generalController.Mood = "anxious";
                break;
        }

        targetMesh = Resources.Load<Mesh>($"{path}{meshName}");
    }

    public void LoadTargetMeshByMood(string mood)
    {
        string path = "Models/Tree/Trunks/";
        string meshName = "";
        switch (mood)
        {
            case "sad":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "SadTrunk";
                break;

            case "neutral":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "NormalTrunk";
                break;

            case "stressed":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "StressedTrunk";
                break;

            case "calm":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

                meshName = "NormalTrunk";
                break;

            case "anxious":
                if (gameObject.tag == "Tree")
                    path = "Models/Tree/Trunks/";

                else if (gameObject.tag == "Bush")
                    path = "Models/Bush/Trunks/";

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

            _generalController.cont++;
            _generalController.CheckVegetationCount();
            
            _leavesController.UpdateLeavesStartColors();
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
            path = "Models/Tree/Trunks";

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
}
