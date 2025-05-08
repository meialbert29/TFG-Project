using UnityEngine;
using UnityEngine.VFX;

public class VFXLODController : MonoBehaviour
{
    public VisualEffect vfxEffect;
    private LODGroup lodGroup;

    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
    }

    void Update()
    {
        if (lodGroup == null || vfxEffect == null) return;

        int currentLOD = GetCurrentLODIndex();
        vfxEffect.enabled = (currentLOD == 0); // Solo activar si es LOD0
    }

    int GetCurrentLODIndex()
    {
        if (Camera.main == null) return -1;
        float relativeHeight = GetRelativeHeight(Camera.main);
        LOD[] lods = lodGroup.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            if (relativeHeight >= lods[i].screenRelativeTransitionHeight)
                return i;
        }
        return lods.Length - 1;
    }

    float GetRelativeHeight(Camera cam)
    {
        float distance = Vector3.Distance(cam.transform.position, transform.position);
        float height = cam.pixelHeight;
        float relHeight = height / (2f * distance * Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad));
        return relHeight;
    }
}
