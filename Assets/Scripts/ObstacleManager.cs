using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ObstacleManager : MonoBehaviour
{
    public ObstacleScriptableObject obstacleScriptableObject;
    public GameObject obstaclePrefab;
    public int gridSpacing = 1;
    [Header("Ensure that X,Z Size Match the ObstacleScriptableObject & GridCreator")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    private GameObject[,] obstacles;

    private void Awake()
    {
        obstacles = new GameObject[xSize, zSize];
    }

    void OnEnable()
    {
        if (obstacleScriptableObject != null) GenerateObstacles();
    }

    public void GenerateObstacles()
    {
        ClearObstacles();
        
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (obstacleScriptableObject.Obstacles[x, z])
                {
                    Vector3 position = new Vector3Int(x * gridSpacing, 1, z * gridSpacing);
                    if (obstacles != null)
                    {
                        obstacles[x, z] = Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
                        obstacles[x, z].GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
        }
    }

    void ClearObstacles()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                if (obstacles[x, z] != null)
                {
                    DestroyImmediate(obstacles[x, z]);
                    obstacles[x, z] = null;
                }
            }
        }
    }
}