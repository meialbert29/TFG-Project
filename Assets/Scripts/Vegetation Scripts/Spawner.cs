using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    public Terrain terrain; // Asigna tu terreno en el Inspector
    [SerializeField] private GeneralController generalController;
    [SerializeField] private VegetationController treePrefab; // Prefab con LOD y VFX
    [SerializeField] private VegetationController bushPrefab; // Prefab con LOD y VFX
    [SerializeField] private GameObject[] rockPrefabs;

    [SerializeField] private int treeCount = 2;
    [SerializeField] private float minDistanceBetweenTrees = 5f;

    [SerializeField] private int bushGroupCount = 10;
    [SerializeField] private int bushIndividualCount = 10;
    [SerializeField] private int bushesPerGroup = 5;
    [SerializeField] private float minDistanceBetweenBushGroups = 10f;
    [SerializeField] private float maxOffsetWithinGroup = 3f;
    [SerializeField] private float minDistanceBetweenBushes = 2f;

    [SerializeField] private int rockCount = 10;
    [SerializeField] private float minDistanceBetweenRocks = 4f;


    private List<Vector3> treePositions = new List<Vector3>();
    private List<Vector3> bushPositions = new List<Vector3>();
    private List<Vector3> bushGroupPositions = new List<Vector3>();
    private List<Vector3> rockPositions = new List<Vector3>();

    bool validPosition = false;

    void Start()
    {
        if(treeCount != 0)
            SpawnTrees();
        if(bushGroupCount != 0)
            SpawnBushGroups();
        if(bushIndividualCount != 0)
            SpawnBushes();
        if (rockCount != 0)
            SpawnRocks();
    }

    void SpawnTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position;
            int attempts = 0;

            do
            {
                position = GetRandomTerrainPosition();

                IsTreePositionValid(position);

                attempts++;
            }
            while (!validPosition && attempts < 10);

            if (validPosition)
            {
                treePositions.Add(position);

                Quaternion treeRotation = Quaternion.identity;
                VegetationController treeInstance = Instantiate(treePrefab, position, treeRotation);
                treeInstance.name = "treeInstance" + i;

                generalController.TreesList.Add(treeInstance);
            }
        }
    }

    void SpawnBushes()
    {
        for (int i = 0; i < bushIndividualCount; i++)
        {
            Vector3 position;
            int attempts = 0;

            do
            {
                position = GetRandomTerrainPosition();

                IsTreePositionValid(position);

                attempts++;
            }
            while (!validPosition && attempts < 10);

            if (validPosition)
            {
                bushPositions.Add(position);

                Quaternion bushRotation = Quaternion.identity;
                VegetationController bushInstance = Instantiate(bushPrefab, position, bushRotation);
                bushInstance.name = "bushInstance" + i;

                generalController.BushesList.Add(bushInstance);
            }
        }
    }


    void SpawnBushGroups()
    {
        List<Vector3> bushGroupCenters = new List<Vector3>();

        for (int g = 0; g < bushGroupCount; g++)
        {
            Vector3 position;
            int attempts = 0;

            do
            {
                position = GetRandomTerrainPosition();

                IsBushesGroupPositionValid(position);

                attempts++;
            }
            while (!validPosition && attempts < 10);

            if (validPosition)
            {
                bushGroupCenters.Add(position);
                bushGroupPositions.Add(position);

                // spawn several bushes around the center
                for (int i = 0; i < bushesPerGroup; i++)
                {
                    Vector2 offset2D = Random.insideUnitCircle * maxOffsetWithinGroup;
                    Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);
                    Vector3 bushPosition = position + offset;
                    bushPosition.y = terrain.SampleHeight(bushPosition) + terrain.transform.position.y;

                    Quaternion bushRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    VegetationController bushInstance = Instantiate(bushPrefab, bushPosition, bushRotation);
                    bushInstance.name = $"bushInstance_g{g}_b{i}";

                    bushPositions.Add(bushPosition);
                    generalController.BushesList.Add(bushInstance);
                }
            }
        }
    }

    void SpawnRocks()
    {
        for (int i = 0; i < rockCount; i++)
        {
            Vector3 position;
            int attempts = 0;

            do
            {
                position = GetRandomTerrainPosition();
                IsRockPositionValid(position);
                attempts++;
            }
            while (!validPosition && attempts < 10);

            if (validPosition)
            {
                rockPositions.Add(position);

                Quaternion rockRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                GameObject selectedRockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

                GameObject rockInstance = Instantiate(selectedRockPrefab, position, rockRotation);
                rockInstance.name = "rockInstance" + i;

                generalController.RocksList.Add(rockInstance);
            }
        }
    }



    private void IsTreePositionValid(Vector3 position)
    {
        validPosition = true;
        foreach (Vector3 treePos in treePositions)
        {
            if (Vector3.Distance(position, treePos) < minDistanceBetweenTrees)
            {
                validPosition = false;
                break;
            }
        }
        foreach (Vector3 bushPos in bushPositions)
        {
            if (Vector3.Distance(position, bushPos) < minDistanceBetweenTrees / 2f)
            {
                validPosition = false;
                break;
            }
        }
        foreach (Vector3 groupCenter in bushGroupPositions)
        {
            if (Vector3.Distance(position, groupCenter) < minDistanceBetweenTrees)
            {
                validPosition = false;
                break;
            }
        }
    }

    private void IsBushPositionValid(Vector3 position)
    {
        validPosition = true;
        foreach (Vector3 bushPos in bushPositions)
        {
            if (Vector3.Distance(position, bushPos) < minDistanceBetweenBushes)
            {
                validPosition = false;
                break;
            }   
        }

        foreach (Vector3 treePos in treePositions)
        {
            if (Vector3.Distance(position, treePos) < minDistanceBetweenTrees / 2f)
            {
                validPosition = false;
                break;
            }
        }
    }

    private void IsBushesGroupPositionValid(Vector3 position)
    {
        validPosition = true;
        foreach (Vector3 otherGroup in bushGroupPositions)
        {
            if (Vector3.Distance(position, otherGroup) < minDistanceBetweenBushGroups)
            {
                validPosition = false;
                break;
            }
        }
        foreach (Vector3 bush in bushPositions)
        {
            if (Vector3.Distance(position, bush) < minDistanceBetweenBushGroups)
            {
                validPosition = false;
                break;
            }
        }
        foreach (Vector3 tree in treePositions)
        {
            if (Vector3.Distance(position, tree) < minDistanceBetweenBushGroups)
            {
                validPosition = false;
                break;
            }
        }
    }

    private Vector3 GetRandomTerrainPosition()
    {
        //Spawn coordinates of the size of the terrain
        float xPos = Random.Range(0, terrain.terrainData.size.x - 5);
        float zPos = Random.Range(0, terrain.terrainData.size.z - 5);

        // Adjust position to the world
        float worldX = terrain.transform.position.x + xPos;
        float worldZ = terrain.transform.position.z + zPos;
        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrain.transform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }

    private void IsRockPositionValid(Vector3 position)
    {
        validPosition = true;

        foreach (Vector3 rockPos in rockPositions)
        {
            if (Vector3.Distance(position, rockPos) < minDistanceBetweenRocks)
            {
                validPosition = false;
                break;
            }
        }

        foreach (Vector3 treePos in treePositions)
        {
            if (Vector3.Distance(position, treePos) < minDistanceBetweenRocks)
            {
                validPosition = false;
                break;
            }
        }

        foreach (Vector3 bushPos in bushPositions)
        {
            if (Vector3.Distance(position, bushPos) < minDistanceBetweenRocks)
            {
                validPosition = false;
                break;
            }
        }

        foreach (Vector3 groupPos in bushGroupPositions)
        {
            if (Vector3.Distance(position, groupPos) < minDistanceBetweenRocks)
            {
                validPosition = false;
                break;
            }
        }
    }

}
