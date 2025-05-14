
// script to control the LOD activity

using UnityEngine;

public class LODController : MonoBehaviour
{
    LODGroup lodGroup;
    LeavesController leavesController;
    public Transform currentLOD;
    Transform lod0;
    Transform lod1;
    Transform lod2;

    Renderer renderer_LOD0;
    Renderer renderer_LOD1;
    Renderer renderer_LOD2;

    void Awake()
    {
        leavesController = GetComponent<LeavesController>();

        lodGroup = GetComponent<LODGroup>();
        lod0 = transform.GetChild(0).GetChild(0);
        lod1 = transform.GetChild(0).GetChild(1);
        lod2 = transform.GetChild(0).GetChild(2);

        renderer_LOD0 = lod0.GetComponent<Renderer>();
        renderer_LOD1 = lod1.GetComponent<Renderer>();
        renderer_LOD2 = lod2.GetComponent<Renderer>();

        //if (renderer_LOD0 != null && renderer_LOD1 != null && renderer_LOD2 != null)
        //    Debug.Log($"Renderer {renderer_LOD0} " +
        //               $"Renderer {renderer_LOD1}" +
        //               $"Renderer {renderer_LOD2}");

        currentLOD = lod1;
    }

    void Update()
    {
        if (renderer_LOD0.isVisible)
        {
            currentLOD = lod0;
        }
        else if (renderer_LOD1.isVisible)
        {
            currentLOD = lod1;
        }
        else if (renderer_LOD2.isVisible)
        {
            currentLOD = lod2;
        }
    }
}
