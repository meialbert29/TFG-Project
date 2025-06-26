using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeavesController : MonoBehaviour
{
    private VegetationController _vb;
    public GeneralController _generalController;

    private LODGroup _lodGroup;
    private LODController _lodController;
    private Material _lod1_Mat;
    private Material _lod2_Mat;
    private Material _trunkMaterial;

    private Color _sad_TopColor          = ColorsPalette.LeavesColors.sad_TopColor;
    private Color _sad_BottomColor       = ColorsPalette.LeavesColors.sad_BottomColor;
    private Color _sad_BlendColor = ColorsPalette.LeavesColors.sad_BlendColor;

    private Color _neutral_TopColor      = ColorsPalette.LeavesColors.neutral_TopColor;
    private Color _neutral_BottomColor   = ColorsPalette.LeavesColors.neutral_BottomColor;
    private Color _neutral_BlendColor = ColorsPalette.LeavesColors.neutral_BlendColor;

    private Color _stressed_TopColor     = ColorsPalette.LeavesColors.stressed_TopColor;
    private Color _stressed_BottomColor  = ColorsPalette.LeavesColors.stressed_BottomColor;
    private Color _stressed_BlendColor = ColorsPalette.LeavesColors.stressed_BlendColor;

    private Color _calm_TopColor          = ColorsPalette.LeavesColors.calm_TopColor;
    private Color _calm_BottomColor       = ColorsPalette.LeavesColors.calm_BottomColor;
    private Color _calm_BlendColor = ColorsPalette.LeavesColors.calm_BlendColor;

    private Color _anxious_TopColor       = ColorsPalette.LeavesColors.anxious_TopColor;
    private Color _anxious_BottomColor    = ColorsPalette.LeavesColors.anxious_BottomColor;
    private Color _anxious_BlendColor = ColorsPalette.LeavesColors.anxious_BlendColor;

    private Color _trunk_StressedColor   = ColorsPalette.TrunkColors.trunk_StressedColor;
    private Color _trunk_NeutralColor    = ColorsPalette.TrunkColors.trunk_NeutralColor;
    private Color _trunk_SadColor        = ColorsPalette.TrunkColors.trunk_SadColor;
    private Color _trunk_AnxiousColor    = ColorsPalette.TrunkColors.trunk_AnxiousColor;
    private Color _trunk_CalmColor       = ColorsPalette.TrunkColors.trunk_CalmColor;
    private Color _target_TrunkColor;

    private Color _start_TopColor;
    private Color _start_BottomColor;
    private Color _start_BlendColor;

    private Color _target_TopColor;
    private Color _target_BottomColor;
    private Color _target_BlendColor;

    private Color _current_TopColor;
    private Color _current_BottomColor;
    private Color current_BlendColor;

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
            string path = "";
            string startMeshName_LOD1 = "";
            string startMeshName_LOD2 = "";
            string targetMeshName_LOD1 = "";
            string targetMeshName_LOD2 = "";

            if(gameObject.tag == "Tree")
                path = "Models/Tree/Leaves/";
            else if(gameObject.tag == "Bush")
                path = "Models/Bush/Leaves/";

            startMeshName_LOD1 = "NormalLeaves_LOD1";
            startMeshName_LOD2 = "NormalLeaves_LOD2";
            targetMeshName_LOD1 = "SadLeaves_LOD1";
            targetMeshName_LOD2 = "SadLeaves_LOD2";

            _startMesh_LOD1 = Resources.Load<Mesh>($"{path}{startMeshName_LOD1}");
            _targetMesh_LOD1 = Resources.Load<Mesh>($"{path}{targetMeshName_LOD1}");
            _startMesh_LOD2 = Resources.Load<Mesh>($"{path}{startMeshName_LOD2}");
            _targetMesh_LOD2 = Resources.Load<Mesh>($"{path}{targetMeshName_LOD2}");
        }

        if (_startMesh_LOD1 == null || _startMesh_LOD2 == null)
        {
            Debug.Log(_startMesh_LOD1);
            Debug.Log(_startMesh_LOD2);
            Debug.LogError("No se pudieron cargar las meshes iniciales.");
            return;
        }

        _meshFilter_LOD1.mesh = _startMesh_LOD1;
        _meshFilter_LOD2.mesh = _startMesh_LOD2;

        // apply neutral by default
        ApplyLeavesColors(_neutral_TopColor, _neutral_BottomColor, _neutral_BlendColor);

        _trunkMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
    }

    void Update()
    {
        _moodType = _generalController.Mood;

        if (_generalController.MoodChanging)
        {
            if (_moodType != _previousMoodType)
            {
                _previousMoodType = _moodType;

                if (_moodType == "sad")
                {
                    _target_TopColor = _sad_TopColor;
                    _target_BottomColor = _sad_BottomColor;
                    _target_BlendColor = _sad_BlendColor;

                    _target_TrunkColor = _trunk_SadColor;
                }
                else if (_moodType == "neutral")
                {
                    _target_TopColor = _neutral_TopColor;
                    _target_BottomColor = _neutral_BottomColor;
                    _target_BlendColor = _neutral_BlendColor;

                    _target_TrunkColor = _trunk_NeutralColor;
                }
                else if (_moodType == "stressed")
                {
                    _target_TopColor = _stressed_TopColor;
                    _target_BottomColor = _stressed_BottomColor;
                    _target_BlendColor = _stressed_BlendColor;

                    _target_TrunkColor = _trunk_StressedColor;
                }
                else if(_moodType == "calm")
                {
                    _target_TopColor = _calm_TopColor;
                    _target_BottomColor = _calm_BottomColor;
                    _target_BlendColor = _calm_BlendColor;

                    _target_TrunkColor = _trunk_CalmColor;
                }
                else if(_moodType == "anxious")
                {
                    _target_TopColor = _anxious_TopColor;
                    _target_BottomColor = _anxious_BottomColor;
                    _target_BlendColor = _anxious_BlendColor;

                    _target_TrunkColor = _trunk_AnxiousColor;
                }

                // save actual state to start
                _start_TopColor = _lod1_Mat.GetColor("_TopColor");
                _start_BottomColor = _lod1_Mat.GetColor("_BottomColor");
                _start_BlendColor = _lod1_Mat.GetColor("_BlendColor");

                _start_TopColor = _lod2_Mat.GetColor("_TopColor");
                _start_BottomColor = _lod2_Mat.GetColor("_BottomColor");
                _start_BlendColor = _lod2_Mat.GetColor("_BlendColor");
            }

            float _progress = _vb.GetTransitionProgress();
            if (_progress >= 0.7f)
            {
                ChangeLeavesLODMeshes();
            }
            UpdateLeavesColor(_progress);
            UpdateTrunkColor(_progress, _target_TrunkColor);
        }
    }

    void UpdateLeavesColor(float progress)
    {
        if(progress < 1f)
        {
            _current_TopColor = Color.Lerp(_start_TopColor, _target_TopColor, progress);
            _current_BottomColor = Color.Lerp(_start_BottomColor, _target_BottomColor, progress);
            current_BlendColor = Color.Lerp(_start_BlendColor, _target_BlendColor, progress);
        }

        ApplyLeavesColors(_current_TopColor, _current_BottomColor, current_BlendColor);
    }

    void ApplyLeavesColors(Color top, Color bottom, Color blend)
    {
        _lod1_Mat.SetColor("_TopColor", top);
        _lod1_Mat.SetColor("_BottomColor", bottom);
        _lod1_Mat.SetColor("_BlendColor", blend);
        _lod2_Mat.SetColor("_TopColor", top);
        _lod2_Mat.SetColor("_BottomColor", bottom);
        _lod2_Mat.SetColor("_BlendColor", blend);
    }
    void ChangeLeavesLODMeshes()
    {
        string path = "";
        string targetMeshName_LOD1 = "";
        string targetMeshName_LOD2 = "";

        if (gameObject.tag == "Tree")
            path = "Models/Tree/Leaves/";
        else if (gameObject.tag == "Bush")
            path = "Models/Bush/Leaves/";

        if(_moodType == "stressed" || _moodType == "anxious")
        {
            _meshFilter_LOD1.mesh = null;
            _meshFilter_LOD2.mesh = null;
        }
        else
        {
            if (_moodType == "sad")
            {
                targetMeshName_LOD1 = "SadLeaves_LOD1";
                targetMeshName_LOD2 = "SadLeaves_LOD2";
            }

            else if (_moodType == "neutral" || _moodType == "calm")
            {
                targetMeshName_LOD1 = "NormalLeaves_LOD1";
                targetMeshName_LOD2 = "NormalLeaves_LOD2";
            }

            _meshFilter_LOD1.mesh = Resources.Load<Mesh>($"{path}{targetMeshName_LOD1}");
            _meshFilter_LOD2.mesh = Resources.Load<Mesh>($"{path}{targetMeshName_LOD2}");
        }
    }

    public void UpdateLeavesStartColors()
    {
        _current_TopColor = _target_TopColor;
        _current_BottomColor = _target_BottomColor;
        current_BlendColor = _target_BlendColor;

        _start_TopColor = _target_TopColor;
        _start_BottomColor = _target_BottomColor;
        _start_BlendColor = _target_BlendColor;
    }

    public void UpdateTrunkColor(float progress, Color targetColor)
    {
        _trunkMaterial.color = Color.Lerp(_trunkMaterial.color, targetColor, progress * 0.05f);
    }
}
