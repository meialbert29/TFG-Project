using UnityEngine;

public class GrassController : MonoBehaviour
{
    private GeneralController _generalController;
    private Material grassMaterial;

    [SerializeField] private Vector2 _nearFarRange;
    [SerializeField] private Color _farColor;
    [SerializeField] private Color _nearColor;
    [SerializeField] private float _smoothness;
    [SerializeField] private float heightBlend;
    [SerializeField] private Color bottomColor;
    [SerializeField] private float alphaThreshold;
    [SerializeField] private float terrainSize;
    [SerializeField] private float terrainOffset;
    [SerializeField] private Texture2D terrainColor;
    [SerializeField] private Color shadowColor;
    [SerializeField] private float windSpeed;
    [SerializeField] private float windIntensity;
    [SerializeField] private Vector2 windNoiseScale;
    [SerializeField] private Vector2 windNoiseSpeed;
    [SerializeField] private Vector2 windNoiseContrast;
    [SerializeField] private Vector2 windHeight;
    
    void Start()
    {
        grassMaterial = GetComponent<Renderer>().material;

        if(grassMaterial == null)
        {
            Debug.Log("Error in Grass Controller");
        }

        else
        {
            _nearFarRange = grassMaterial.GetVector("_NearFarRange");
            _farColor = grassMaterial.GetColor("_FarColor");
            _nearColor = grassMaterial.GetColor("_NearColor");
            _smoothness = grassMaterial.GetFloat("_Smoothness");
            heightBlend = grassMaterial.GetFloat("_HeightBlend");
            bottomColor = grassMaterial.GetColor("_BottomColor");
            alphaThreshold = grassMaterial.GetFloat("_Alpha Threshold");
            terrainSize = grassMaterial.GetFloat("_TerrainSize");
            terrainOffset = grassMaterial.GetFloat("_TerrainOffset");
            terrainColor = grassMaterial.GetTexture("_TerrainColor") as Texture2D;


        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
