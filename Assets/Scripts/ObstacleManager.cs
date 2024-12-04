using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GridCreator gridCreator;
    [SerializeField] private ObstacleScriptableObject obstacleScriptableObject;
    [Header("Ensure that X,Z Size Match the ObstacleScriptableObject & GridCreator")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    public int gridSpacing = 1;
    [SerializeField] private GameObject obstaclePrefab;
    
    
    private GameObject[,] obstacles;

    
    private void Awake()
    {
        obstacles = new GameObject[xSize, zSize];
    }

    // Generate obstacles when the script is enabled
    private void OnEnable()
    {
        if (obstacleScriptableObject != null) GenerateObstacles();
    }

    // Generate obstacles based on the ObstacleScriptableObject data
    public void GenerateObstacles()
    {
        ClearObstacles();

        //null check for obstacles and obstacleScriptableObject and its obstacles
        if (obstacles == null || obstacleScriptableObject == null || obstacleScriptableObject.obstaclesToggles == null) return;

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                // Get the tile at the current position and set the obstacle
                var currentGridTile = GetTileAt(x, z);
                if (currentGridTile != null)
                    currentGridTile.SetObstacle(obstacleScriptableObject.obstaclesToggles[x, z]);

                if (obstacleScriptableObject.obstaclesToggles[x, z])
                {
                    Vector3Int obstaclePosition = new Vector3Int(x * gridSpacing, 1, z * gridSpacing);
                    obstacles[x, z] = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, transform);
                }
            }
        }
    }

    
    // Get the tile at the specified grid position (x, z)
    private GridTile GetTileAt(int x, int z)
    {
        foreach (var gridTile in gridCreator.gridTiles)
        {
            var tile = gridTile.GetComponent<GridTile>();
            if (tile.GridX == x && tile.GridZ == z)
            {
                return tile;
            }
        }
        return null;
    }
    
    //Ensure that the obstacles are cleared before generating new ones
    private void ClearObstacles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (obstacles[x, z] != null)
                {
                    Destroy(obstacles[x, z]);
                    obstacles[x, z] = null;
                }
            }
        }
    }
}