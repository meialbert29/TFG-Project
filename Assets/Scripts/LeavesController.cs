using UnityEngine;

public class LeavesController : MonoBehaviour
{
    private VegetationBehaviour _vegetation;
    private LODGroup _lodGroup;
    private LODController _lodController;
    private Material _lod1_Mat;
    private Material _lod2_Mat;
    private Material _trunkMaterial;

    private Color _sad_TopColor = new Color(0.7735849f, 0.4542985f, 0.2262369f, 1f);
    private Color _sad_BottomColor = new Color(0.8805031f, 0.6550949f, 0.3405719f, 1f);
    private Color _neutral_TopColor = new Color(0.5644949f, 0.8930817f, 0.582952f, 1f);
    private Color _neutral_BottomColor = new Color(0.5430757f, 0.7106918f, 0.2346623f, 1f);
    private Color _stressed_TopColor = new Color(0.06607719f, 0.06925166f, 0.08176088f, 1f);
    private Color _stressed_BottomColor = new Color(0.1509434f, 0.1509434f, 0.1509434f, 1f);

    private Color _trunk_StressedColor = new Color(0f, 0f, 0f, 1f);
    private Color _trunk_NeutralColor = new Color(0.8396226f, 0.7110994f, 0.5425863f, 1f);
    private Color _trunk_SadColor = new Color(0.3396226f, 0.2771448f, 0.2771448f, 1f);
    private Color _target_TrunkColor;

    private Color _start_TopColor;
    private Color _start_BottomColor;
    private Color _target_TopColor;
    private Color _target_BottomColor;
    private Color _current_TopColor;
    private Color _current_BottomColor;

    private string _moodType;
    private string _previousMoodType = "";

    private Mesh _startMesh_LOD1;
    private Mesh _targetMesh_LOD1;
    private Mesh _startMesh_LOD2;
    private Mesh _targetMesh_LOD2;
    private MeshFilter _meshFilter_LOD1;
    private MeshFilter _meshFilter_LOD2;

    void Start()
    {
        _vegetation = GetComponent<VegetationBehaviour>();
        _lodGroup = GetComponent<LODGroup>();
        _lodController = GetComponent<LODController>();

        Transform _lod1 = transform.GetChild(0).GetChild(1);
        Transform _lod2 = transform.GetChild(0).GetChild(2);

        _lod1_Mat = _lod1.GetComponent<Renderer>().material;
        _lod2_Mat = _lod2.GetComponent<Renderer>().material;

        _meshFilter_LOD1 = _lod1.GetComponent<MeshFilter>();
        _meshFilter_LOD2 = _lod2.GetComponent<MeshFilter>();

        if (gameObject.layer == LayerMask.NameToLayer("Vegetation") && _lod1.tag == "Leaves" && _lod2.tag == "Leaves")
        {
            _startMesh_LOD1 = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD1");
            _targetMesh_LOD1 = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD1");

            _startMesh_LOD2 = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD2");
            _targetMesh_LOD2 = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD2");
        }

        if (_startMesh_LOD1 == null || _startMesh_LOD2 == null)
        {
            Debug.LogError("No se pudieron cargar las meshes iniciales.");
            return;
        }

        _meshFilter_LOD1.mesh = _startMesh_LOD1;
        _meshFilter_LOD2.mesh = _startMesh_LOD2;

        // Por defecto, aplicamos neutral
        applyLeavesColors(_neutral_TopColor, _neutral_BottomColor);

        _trunkMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
    }

    void Update()
    {
        _moodType = _vegetation.mood;

        if (_moodType != _previousMoodType)
        {
            _previousMoodType = _moodType;

            if (_moodType == "sad")
            {
                _target_TopColor = _sad_TopColor;
                _target_BottomColor = _sad_BottomColor;
                _target_TrunkColor = _trunk_SadColor;
            }
            else if (_moodType == "neutral")
            {
                _target_TopColor = _neutral_TopColor;
                _target_BottomColor = _neutral_BottomColor;
                _target_TrunkColor = _trunk_NeutralColor;
            }
            else if(_moodType == "stressed")
            {
                _target_TopColor = _stressed_TopColor;
                _target_BottomColor = _stressed_BottomColor;
                _target_TrunkColor = _trunk_StressedColor;
            }

            // Guardamos el estado actual como punto de partida
            _start_TopColor = _lod1_Mat.GetColor("_TopColor");
            _start_BottomColor = _lod1_Mat.GetColor("_BottomColor");
        }

        float _progress = _vegetation.getTransitionProgress();
        if(_progress >= 0.7f)
        {
            changeLeavesLODMeshes();
        }
        updateLeavesColor(_progress);
        updateTrunkColor(_progress, _target_TrunkColor);
    }

    void updateLeavesColor(float progress)
    {
        if(progress < 1f)
        {
            _current_TopColor = Color.Lerp(_start_TopColor, _target_TopColor, progress);
            _current_BottomColor = Color.Lerp(_start_BottomColor, _target_BottomColor, progress);
        }

        applyLeavesColors(_current_TopColor, _current_BottomColor);
    }

    void applyLeavesColors(Color top, Color bottom)
    {
        _lod1_Mat.SetColor("_TopColor", top);
        _lod1_Mat.SetColor("_BottomColor", bottom);
        _lod2_Mat.SetColor("_TopColor", top);
        _lod2_Mat.SetColor("_BottomColor", bottom);
    }
    void changeLeavesLODMeshes()
    {

        if(_moodType == "sad")
        {
            _meshFilter_LOD1.mesh = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD1");
            _meshFilter_LOD2.mesh = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD2");
        }

        else if(_moodType == "neutral")
        {
            _meshFilter_LOD1.mesh = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD1");
            _meshFilter_LOD1.mesh = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD2");
        }
        else if(_moodType == "stressed")
        {
            _meshFilter_LOD1.mesh = null;
            _meshFilter_LOD2.mesh = null;
        }
    }

    public void updateLeavesStartColors()
    {
        _current_TopColor = _target_TopColor;
        _current_BottomColor = _target_BottomColor;

        _start_TopColor = _target_TopColor;
        _start_BottomColor = _target_BottomColor;
    }

    public void updateTrunkColor(float progress, Color targetColor)
    {

        _trunkMaterial.color = Color.Lerp(_trunkMaterial.color, targetColor, progress * 0.05f);
    }
}
