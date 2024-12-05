using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteAlways]
public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GridCreator gridCreator;
    [SerializeField] private ObstacleScriptableObject obstacleData;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private int gridSpacing = 1;
    
    private GameObject[,] obstacleInstances;
    private bool needsRegeneration = false;

    private void OnEnable()
    {
        if (obstacleData != null)
        {
            InitializeObstacleArray();
            RegenerateObstacles();
        }
    }

    private void Update()
    {
        if (needsRegeneration)
        {
            RegenerateObstacles();
            needsRegeneration = false;
        }
    }

    private void OnValidate()
    {
        if (obstacleData != null)
        {
            InitializeObstacleArray();
            needsRegeneration = true;
        }
    }

    private void OnDisable()
    {
        ClearAllObstacles();
    }

    private void InitializeObstacleArray()
    {
        if (obstacleInstances == null || 
            obstacleInstances.GetLength(0) != obstacleData.XSize || 
            obstacleInstances.GetLength(1) != obstacleData.ZSize)
        {
            ClearAllObstacles();
            obstacleInstances = new GameObject[obstacleData.XSize, obstacleData.ZSize];
        }
    }

    public void RegenerateObstacles()
    {
        if (!IsSetupValid()) return;
        
        if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // Check if we are in prefab stage editor
            bool isInPrefabStage = false;
            #if UNITY_EDITOR
            isInPrefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null; 
            #endif
            
            if (!isInPrefabStage)
            {
                EditorApplication.delayCall += () => {
                    if (this != null)
                    {
                        GenerateObstacles();
                    }
                };
                return;
            }
        }
        
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        if (!IsSetupValid()) return;

        ClearAllObstacles();
        
        for (int x = 0; x < obstacleData.XSize; x++)
        {
            for (int z = 0; z < obstacleData.ZSize; z++)
            {
                UpdateObstacleAt(x, z);
            }
        }
    }

    private bool IsSetupValid()
    {
        return obstacleData != null && 
               obstaclePrefab != null && 
               gridCreator != null && 
               obstacleData.obstacleGrid != null;
    }

    private void UpdateObstacleAt(int x, int z)
    {
        var gridTile = GetTileAt(x, z);
        if (gridTile != null)
        {
            gridTile.SetObstacle(obstacleData.obstacleGrid[x, z]);
        }

        if (obstacleData.obstacleGrid[x, z])
        {
            CreateObstacleAt(x, z);
        }
    }

    private void CreateObstacleAt(int x, int z)
    {
        Vector3 position = new Vector3(x * gridSpacing, 1f, z * gridSpacing);
        obstacleInstances[x, z] = Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
        obstacleInstances[x, z].name = $"Obstacle ({x}, {z})";
    }

    private GridTile GetTileAt(int x, int z)
    {
        if (gridCreator?.gridTiles == null) return null;
        
        foreach (var tileObject in gridCreator.gridTiles)
        {
            if (tileObject == null) continue;
            
            var tile = tileObject.GetComponent<GridTile>();
            if (tile != null && tile.GridX == x && tile.GridZ == z)
            {
                return tile;
            }
        }
        return null;
    }

    private void ClearAllObstacles()
    {
        // First, clear the tracked obstacles
        if (obstacleInstances != null)
        {
            for (int x = 0; x < obstacleInstances.GetLength(0); x++)
            {
                for (int z = 0; z < obstacleInstances.GetLength(1); z++)
                {
                    if (obstacleInstances[x, z] != null)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(obstacleInstances[x, z]);
                        }
                        else
                        {
                            DestroyImmediate(obstacleInstances[x, z]);
                        }
                        obstacleInstances[x, z] = null;
                    }
                }
            }
        }
        
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}