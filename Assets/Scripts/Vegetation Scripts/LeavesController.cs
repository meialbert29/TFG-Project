using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavesController : MonoBehaviour
{
    private VegetationController _vb;
    public GeneralController _gc;

    private LODGroup _lodGroup;
    private LODController _lodController;
    private Material _lod1_Mat;
    private Material _lod2_Mat;
    private Material _trunkMaterial;

    private Color _sad_TopColor          = ColorsPalette.LeavesColors.sad_TopColor;
    private Color _sad_BottomColor       = ColorsPalette.LeavesColors.sad_BottomColor;
    private Color _neutral_TopColor      = ColorsPalette.LeavesColors.neutral_TopColor;
    private Color _neutral_BottomColor   = ColorsPalette.LeavesColors.neutral_BottomColor;
    private Color _stressed_TopColor     = ColorsPalette.LeavesColors.stressed_TopColor;
    private Color _stressed_BottomColor  = ColorsPalette.LeavesColors.stressed_BottomColor;

    private Color _trunk_StressedColor   = ColorsPalette.TrunkColors.trunk_StressedColor;
    private Color _trunk_NeutralColor    = ColorsPalette.TrunkColors.trunk_NeutralColor;
    private Color _trunk_SadColor        = ColorsPalette.TrunkColors.trunk_SadColor;
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
        _vb = transform.GetComponent<VegetationController>();
        _lodGroup = transform.GetComponent<LODGroup>();
        _lodController = transform.GetComponent<LODController>();

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
        ApplyLeavesColors(_neutral_TopColor, _neutral_BottomColor);

        _trunkMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
    }

    void Update()
    {
        _moodType = _gc.Mood;

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

        float _progress = _vb.GetTransitionProgress();
        if(_progress >= 0.7f)
        {
            ChangeLeavesLODMeshes();
        }
        UpdateLeavesColor(_progress);
        UpdateTrunkColor(_progress, _target_TrunkColor);
    }

    void UpdateLeavesColor(float progress)
    {
        if(progress < 1f)
        {
            _current_TopColor = Color.Lerp(_start_TopColor, _target_TopColor, progress);
            _current_BottomColor = Color.Lerp(_start_BottomColor, _target_BottomColor, progress);
        }

        ApplyLeavesColors(_current_TopColor, _current_BottomColor);
    }

    void ApplyLeavesColors(Color top, Color bottom)
    {
        _lod1_Mat.SetColor("_TopColor", top);
        _lod1_Mat.SetColor("_BottomColor", bottom);
        _lod2_Mat.SetColor("_TopColor", top);
        _lod2_Mat.SetColor("_BottomColor", bottom);
    }
    void ChangeLeavesLODMeshes()
    {

        if(_moodType == "sad")
        {
            _meshFilter_LOD1.mesh = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD1");
            _meshFilter_LOD2.mesh = Resources.Load<Mesh>("Models/Leaves/SadLeaves_LOD2");
        }

        else if(_moodType == "neutral")
        {
            _meshFilter_LOD1.mesh = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD1");
            _meshFilter_LOD2.mesh = Resources.Load<Mesh>("Models/Leaves/NormalLeaves_LOD2");
        }
        else if(_moodType == "stressed")
        {
            _meshFilter_LOD1.mesh = null;
            _meshFilter_LOD2.mesh = null;
        }
    }

    public void UpdateLeavesStartColors()
    {
        _current_TopColor = _target_TopColor;
        _current_BottomColor = _target_BottomColor;

        _start_TopColor = _target_TopColor;
        _start_BottomColor = _target_BottomColor;
    }

    public void UpdateTrunkColor(float progress, Color targetColor)
    {
        _trunkMaterial.color = Color.Lerp(_trunkMaterial.color, targetColor, progress * 0.05f);
    }
}
