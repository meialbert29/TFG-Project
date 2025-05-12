using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeSpawner : MonoBehaviour
{
    public Terrain terrain; // Asigna tu terreno en el Inspector
    public VegetationBehaviour treePrefab; // Prefab con LOD y VFX

    public int treeCount = 2;
    public float minDistanceBetweenTrees = 5f;

    private List<Vector3> treePositions = new List<Vector3>();

    private VegetationBehaviour spawnedTree;


    void Start()
    {
        //SpawnTrees();

        float xPos = Random.Range(0, terrain.terrainData.size.x);
        float zPos = Random.Range(0, terrain.terrainData.size.z);

        // Ajusta la posición al mundo
        float worldX = terrain.transform.position.x + xPos;
        float worldZ = terrain.transform.position.z + zPos;
        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrain.transform.position.y;

        Vector3 spawnPosition = new Vector3(worldX, worldY, worldZ);

        spawnedTree = Instantiate(treePrefab, spawnPosition, Quaternion.identity);
        spawnedTree.transform.localScale = Vector3.one; // Asegura escala correcta

        spawnedTree.name = "treeInstance";

        AnalyzeLODGroup(spawnedTree);
    }

    void SpawnTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position;
            int attempts = 0;
            bool validPosition = false;

            do
            {
                // Genera coordenadas dentro del tamaño del terreno
                float xPos = Random.Range(0, terrain.terrainData.size.x);
                float zPos = Random.Range(0, terrain.terrainData.size.z);

                // Ajusta la posición al mundo
                float worldX = terrain.transform.position.x + xPos;
                float worldZ = terrain.transform.position.z + zPos;
                float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrain.transform.position.y;

                position = new Vector3(worldX, worldY, worldZ);

                // Verifica que no esté demasiado cerca de otros árboles
                validPosition = true;
                foreach (Vector3 existingPosition in treePositions)
                {
                    if (Vector3.Distance(position, existingPosition) < minDistanceBetweenTrees)
                    {
                        validPosition = false;
                        break;
                    }
                }

                attempts++;
            }
            while (!validPosition && attempts < 10);

            if (validPosition)
            {
                treePositions.Add(position);

                Quaternion treeRotation = Quaternion.identity;
                VegetationBehaviour treeInstance = Instantiate(treePrefab, position, treeRotation);

                // Asegura la escala correcta (útil si el prefab tiene escala guardada diferente)
                treeInstance.transform.localScale = Vector3.one;

                Debug.Log("Escala del objeto: " + treeInstance.transform.localScale);
            }
        }

    }

    void AnalyzeLODGroup(VegetationBehaviour tree)
    {
        LODGroup lodGroup = tree.GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            Debug.Log("Este objeto no tiene un LODGroup.");
            return;
        }

        LOD[] lods = lodGroup.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            Debug.Log($"--- LOD {i} ---");

            foreach (Renderer renderer in lods[i].renderers)
            {
                if (renderer == null)
                {
                    Debug.LogWarning($"LOD {i}: renderer nulo");
                    continue;
                }

                Mesh mesh = null;

                if (renderer is MeshRenderer)
                {
                    MeshFilter mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null)
                        mesh = mf.sharedMesh;
                }
                else if (renderer is SkinnedMeshRenderer skinned)
                {
                    mesh = skinned.sharedMesh;
                }

                if (mesh != null)
                {
                    Debug.Log($"LOD {i}: Mesh encontrada: '{mesh.name}' en objeto '{renderer.gameObject.name}'");
                }
                else
                {
                    Debug.Log($"LOD {i}: Sin malla en renderer '{renderer.gameObject.name}'");
                }
            }
        }
    }
}
