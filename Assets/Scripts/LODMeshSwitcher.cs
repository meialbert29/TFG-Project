using UnityEngine;

[RequireComponent(typeof(LODGroup))]
public class LODMeshSwitcher : MonoBehaviour
{
    private LODGroup lodGroup;
    private MeshFilter meshFilter;
    private LOD[] lods;
    private int currentLODIndex = -1;

    public MeshFilter lodMeshFilter;

    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        meshFilter = GetComponent<MeshFilter>();
        lods = lodGroup.GetLODs();
    }

    void Update()
    {
        int lodIndex = GetCurrentLODIndex();

        if (lodIndex != currentLODIndex)
        {
            currentLODIndex = lodIndex;
            UpdateMeshFromLOD(currentLODIndex);
        }
    }

    int GetCurrentLODIndex()
    {
        float relativeHeight = GetRelativeHeight(Camera.main, lodGroup);

        for (int i = 0; i < lods.Length; i++)
        {
            if (relativeHeight >= lods[i].screenRelativeTransitionHeight)
                return i;
        }

        return lods.Length - 1; // fallback to lowest LOD
    }

    float GetRelativeHeight(Camera camera, LODGroup group)
    {
        float distance = Vector3.Distance(camera.transform.position, transform.position);
        float halfHeight = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f) * distance;
        float screenHeight = halfHeight * 2f;
        float objectHeight = GetComponent<Renderer>().bounds.size.y;
        return objectHeight / screenHeight;
    }

    public void UpdateMeshFromLOD(int lodIndex)
    {
        if (lodIndex < 0 || lodIndex >= lods.Length) return;

        Renderer lodRenderer = lods[lodIndex].renderers[0]; // suponiendo un solo renderer por LOD

        MeshFilter lodMeshFilter = lodRenderer.GetComponent<MeshFilter>();
        if (lodMeshFilter != null)
        {
            meshFilter.mesh = lodMeshFilter.sharedMesh;
            Debug.Log("Switched to LOD " + lodIndex + ": " + lodMeshFilter.sharedMesh.name);
        }
    }
}
