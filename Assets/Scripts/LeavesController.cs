using UnityEngine;

public class LeavesController : MonoBehaviour
{
    private VegetationBehaviour vegetation;
    private LODGroup lodGroup;
    private LODController lodController;
    private Material lod1_Mat;
    private Material lod2_Mat;

    private Color sadTopColor = new Color(0.7735849f, 0.4542985f, 0.2262369f, 1f);
    private Color sadBottomColor = new Color(0.8805031f, 0.6550949f, 0.3405719f, 1f);
    private Color neutralTopColor = new Color(0.5644949f, 0.8930817f, 0.582952f, 1f);
    private Color neutralBottomColor = new Color(0.5430757f, 0.7106918f, 0.2346623f, 1f);

    private Color startTopColor;
    private Color startBottomColor;
    private Color targetTopColor;
    private Color targetBottomColor;
    private Color currentTopColor;
    private Color currentBottomColor;

    private string moodType;
    private string previousMoodType = "";

    private Mesh startMesh_LOD1;
    private Mesh targetMesh_LOD1;
    private Mesh startMesh_LOD2;
    private Mesh targetMesh_LOD2;
    private MeshFilter meshFilter_LOD1;
    private MeshFilter meshFilter_LOD2;

    void Start()
    {
        vegetation = GetComponent<VegetationBehaviour>();
        lodGroup = GetComponent<LODGroup>();
        lodController = GetComponent<LODController>();

        Transform lod1 = transform.GetChild(0).GetChild(1);
        Transform lod2 = transform.GetChild(0).GetChild(2);

        lod1_Mat = lod1.GetComponent<Renderer>().material;
        lod2_Mat = lod2.GetComponent<Renderer>().material;

        meshFilter_LOD1 = lod1.GetComponent<MeshFilter>();
        meshFilter_LOD2 = lod2.GetComponent<MeshFilter>();

        if (gameObject.layer == LayerMask.NameToLayer("Vegetation") && lod1.tag == "Leaves" && lod2.tag == "Leaves")
        {
            startMesh_LOD1 = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD1");
            targetMesh_LOD1 = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD1");

            startMesh_LOD2 = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD2");
            targetMesh_LOD2 = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD2");
        }

        if (startMesh_LOD1 == null || startMesh_LOD2 == null)
        {
            Debug.LogError("No se pudieron cargar las meshes iniciales.");
            return;
        }

        meshFilter_LOD1.mesh = startMesh_LOD1;
        meshFilter_LOD2.mesh = startMesh_LOD2;

        // Por defecto, aplicamos neutral
        applyColors(neutralTopColor, neutralBottomColor);
    }

    void Update()
    {
        moodType = vegetation.mood;

        if (moodType != previousMoodType)
        {
            previousMoodType = moodType;

            // Cuando cambia el estado, definimos nuevos colores objetivo
            if (moodType == "sad")
            {
                targetTopColor = sadTopColor;
                targetBottomColor = sadBottomColor;
            }
            else if (moodType == "neutral")
            {
                targetTopColor = neutralTopColor;
                targetBottomColor = neutralBottomColor;
            }

            // Guardamos el estado actual como punto de partida
            startTopColor = lod1_Mat.GetColor("_TopColor");
            startBottomColor = lod1_Mat.GetColor("_BottomColor");
        }

        float progress = vegetation.getTransitionProgress();
        if(progress >= 0.7f)
        {
            changeLODMeshes(targetMesh_LOD1 , targetMesh_LOD2);
        }
        updateColorsOverTime(progress);
    }

    void updateColorsOverTime(float progress)
    {
        if(progress < 1f)
        {
            currentTopColor = Color.Lerp(startTopColor, targetTopColor, progress);
            currentBottomColor = Color.Lerp(startBottomColor, targetBottomColor, progress);
        }

        else
        {
            Debug.Log("enter");
            // Fijamos el color final y lo usamos como nuevo punto de partida
            
        }

        applyColors(currentTopColor, currentBottomColor);
    }

    void applyColors(Color top, Color bottom)
    {
        lod1_Mat.SetColor("_TopColor", top);
        lod1_Mat.SetColor("_BottomColor", bottom);
        lod2_Mat.SetColor("_TopColor", top);
        lod2_Mat.SetColor("_BottomColor", bottom);
    }
    void changeLODMeshes(Mesh meshLOD1, Mesh meshLOD2)
    {
        meshFilter_LOD1.mesh = meshLOD1;
        meshFilter_LOD2.mesh = meshLOD2;
    }

    public void updateStartColorsFromCurrentColors()
    {
        currentTopColor = targetTopColor;
        currentBottomColor = targetBottomColor;

        startTopColor = targetTopColor;
        startBottomColor = targetBottomColor;
    }
}
