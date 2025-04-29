using Assets.LSL4Unity.Scripts.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VegetationBehaviour : MonoBehaviour
{
    private Renderer vegetationRenderer;
    //private environmentcontroller environmentcontroller;
    //private playerfindsclosest playerfindsclosest;
    private ExampleFloatInlet eeg_script;

    // Variables para onda
    private string currentWave;  // Almacena la onda actual
    private float waveConsistencyTimer = 0f;  // Contador para la consistencia de la onda
    private const float waveConsistencyDuration = 5f;  // Duración para verificar la consistencia de la onda en segundos
    private bool isWaveConsistent = false;  // Bandera para saber si la onda ha sido consistente

    //MESHES
    private string actualState;
    private string latestState;
    private string actualMesh;
    private MeshFilter meshFilter;
    private Mesh startMesh;
    private Mesh targetMesh;
    private Mesh morphedMesh;

    //Leaves
    private VFXController vfx;
    float currentLeavesVisibility;
    float latestLeavesVisibility;

    //PROGRESS
    private float transitionProgress = 0f;
    private float transitionDuration = 3f;

    //BOOLS
    private bool isMorphing = false;
    private bool alreadyChanged = false;
    private bool firstTime = true;
    private bool isTransitioning = false;

    //OTHERS
    public List<VegetationBehaviour> neighbourVegetation;
    //private float radio;
    private string vegetationType;

    void Start()
    {
        // find scripts
        //playerFindsClosest = FindAnyObjectByType<PlayerFindsClosest>();
        //if (playerFindsClosest == null) Debug.Log("PlayerFindsClosest Script not found");

        //environmentController = FindAnyObjectByType<EnvironmentController>();
        //if (environmentController == null) Debug.Log("EnvironmentController Script not found");

        eeg_script = FindAnyObjectByType<ExampleFloatInlet>();
        if (eeg_script == null) Debug.Log("EEG Script not found");

        vfx = GetComponentInChildren<VFXController>();
        if (vfx == null) Debug.Log("VFX Graph not found");
        else Debug.Log("VFX found");

        meshFilter = GetComponentInChildren<MeshFilter>();
        if (vfx == null) Debug.Log("Mesh filter not found");
        else Debug.Log("Mesh filter found: " + meshFilter);

        vegetationRenderer = GetComponent<Renderer>();
        vegetationType = gameObject.tag;

        if (vegetationType == "Tree")
        {
            startMesh = Resources.Load<Mesh>("Models/NormalTrunk");
            targetMesh = Resources.Load<Mesh>("Models/SadTrunk");

            //radio = 7f;
        }

        else if (vegetationType == "Plant")
        {
            startMesh = Resources.Load<Mesh>("Meshes/Plants/NeutralPlant");
            targetMesh = Resources.Load<Mesh>("Meshes/Plants/NeutralPlant");

            //radio = 5f;
        }

        if (startMesh == null || targetMesh == null)
        {
            Debug.LogError("Meshes were not loaded correctly. Please check the path.");
            return;
        }

        // initialize morph mesh attributes
        morphedMesh = new Mesh();
        meshFilter.mesh = startMesh;
        morphedMesh.vertices = startMesh.vertices;
        morphedMesh.triangles = startMesh.triangles;
        morphedMesh.normals = startMesh.normals;
        morphedMesh.uv = startMesh.uv;

        //neighbourVegetation = new List<VegetationBehaviour>();
        //FindNeighbours();
    }

    void Update()
    {
        HandleWaveConsistency();
        if (isMorphing){
            vfx.fall = true;
            MorphingProcess();
        }
           
    }

    private void HandleWaveConsistency()
    {
        // get current wave
        string newWave = GetCurrentWave();

        if (newWave == currentWave)
        {
            waveConsistencyTimer += Time.deltaTime;

            // Si la onda ha sido consistente durante 10 segundos, activamos el morphing
            if (waveConsistencyTimer >= waveConsistencyDuration && !isWaveConsistent)
            {
                Debug.Log("Wave consistent");
                isWaveConsistent = true;
                isMorphing = true;
            }
        }
        else
        {
            waveConsistencyTimer = 0f;  // Reiniciamos el contador si la onda cambia
            isWaveConsistent = false;  // La onda no es consistente
            currentWave = newWave;  // Actualizamos la onda actual

            MorphingByWave(currentWave);
        }
    }

    private string GetCurrentWave()
    {
        // Aquí es donde obtienes el tipo de onda actual, puede ser de tu sistema de EEG o de otra fuente
        return eeg_script.lastWaveType;
    }

    private void MorphingByWave(string wave)
    {
        switch (wave)
        {
            case "Delta":
                targetMesh = Resources.Load<Mesh>("Models/Sad Tree/SadTree");
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
                targetMesh = Resources.Load<Mesh>("Models/Meshes/Tree Trunk/NeutralTree");
                break;
        }
    }

    public void MorphingProcess()
    {
        // add time to transitionProgress
        transitionProgress += Time.deltaTime / transitionDuration;

        if (transitionProgress >= 0.5f)
        {
            vfx.isSad = true;
            vfx.changeMood();
        }

        if (transitionProgress > 1f)
        {
            transitionProgress = 1f;
            startMesh = targetMesh;
            isMorphing = false;
            vfx.fall = false;

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

        if (vegetationType == "Tree")
            path = "Meshes/Tree Trunk/";
        //else if (vegetationType == "Plant")
        //    path = "Meshes/Plants/";
        //else if (vegetationType == "Bush")
        //    path = "Meshes/Bushes/";

        targetMesh = Resources.Load<Mesh>($"{path}{meshName}");
        transitionProgress = 0f;
    }

    private void StartMorphing()
    {
        // Inicia el morphing solo si la onda ha sido consistente durante 10 segundos
        Debug.Log("Onda consistente durante 10 segundos. Activando morphing.");
        isMorphing = true;  // Esto activa el morphing
    }


    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            actualState = "sad";
            if (vegetationType == "Plant")
                actualMesh = "SadPlant";
            else if (vegetationType == "Tree")
                actualMesh = "SadTree";
            else if (vegetationType == "Bush")
                actualMesh = "SadBush";

            alreadyChanged = false;
            firstTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            actualState = "neutral";
            if (vegetationType == "Plant")
                actualMesh = "NeutralPlant";
            else if (vegetationType == "Tree")
                actualMesh = "NeutralTree";
            else if (vegetationType == "Bush")
                actualMesh = "NeutralBush";

            alreadyChanged = false;
            firstTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            actualState = "happy";
            if (vegetationType == "Plant")
                actualMesh = "HappyPlant";
            else if (vegetationType == "Tree")
                actualMesh = "HappyTree";
            else if (vegetationType == "Bush")
                actualMesh = "HappyBush";

            alreadyChanged = false;
            firstTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            actualState = "angry";
            if (vegetationType == "Plant")
                actualMesh = "AngryPlant";
            else if (vegetationType == "Tree")
                actualMesh = "AngryTree";
            else if (vegetationType == "Bush")
                actualMesh = "AngryBush";

            alreadyChanged = false;
            firstTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            actualState = "anxiety";
            if (vegetationType == "Plant")
                actualMesh = "AnxiousPlant";
            else if (vegetationType == "Tree")
                actualMesh = "AnxiousTree";
            else if (vegetationType == "Bush")
                actualMesh = "AnxiousBush";

            alreadyChanged = false;
            firstTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            actualState = "stress";
            if (vegetationType == "Plant")
                actualMesh = "StressPlant";
            else if (vegetationType == "Tree")
                actualMesh = "StressTree";
            else if (vegetationType == "Bush")
                actualMesh = "StressBush";

            alreadyChanged = false;
            firstTime = false;
        }
    }

    //private void FindNeighbours()
    //{
    //    float distance;
    //    foreach (VegetationBehaviour vb in playerFindsClosest.allVegetation)
    //    {
    //        if (vb != this)
    //        {
    //            distance = Vector3.Distance(this.transform.position, vb.transform.position);
    //            if (distance < radio)
    //            {
    //                neighbourVegetation.Add(vb); //Add closest vegetation to list
    //            }
    //        }
    //    }
    //}

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

        // Initializes the interpolation of the neighbours
        foreach (VegetationBehaviour neighbour in neighbourVegetation)
        {

            if (!neighbour.alreadyChanged)
                neighbour.MorphNeighbours(neighbour);
        }
    }
}
