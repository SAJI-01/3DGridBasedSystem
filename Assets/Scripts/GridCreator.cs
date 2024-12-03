using System;
using System.Collections.Generic;
using UnityEngine;


public class GridCreator : MonoBehaviour
{
    // Reference to the UIManager script
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject highlightEffectGameObject;
    [SerializeField] private int gridSpacing = 1;
    [SerializeField] private float highlightPositionY = 0.51f;
    public GameObject[,] grid;
    public List<GameObject> gridTiles;

    [Space] 
    [Header("Ensure that X,Z Size Match the ObstacleManager & ObstacleScriptableObject")] 
    [SerializeField] public int xSize = 10;
    [SerializeField] public int zSize = 10;

    public float nodeDiameter = 1f; // Define node diameter

    private void Start()
    {
        // Ensure that the highlight effect is disabled so visible only when needed
        highlightEffectGameObject.SetActive(false);
        CreateGrid();
    }
    
    // Create the grid of tiles
    private void CreateGrid()
    {
        
        gridTiles ??= new List<GameObject>();
        grid = new GameObject[xSize, zSize];  
        EnsureNoDuplicateTiles();

        for (var x = 0; x < xSize; x++)
        {
            for (var z = 0; z < zSize; z++)
            {
                var position = new Vector3(x * gridSpacing, 0, z * gridSpacing);
                var gridTile = Instantiate(tilePrefab, position, Quaternion.identity);
                var boxCollider = gridTile.AddComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    Debug.LogError("Failed to add BoxCollider to the tile.");
                }
                gridTile.AddComponent<GridTile>().SetPosition(x, z);
                // Settings the name of the tile to its position
                gridTile.transform.name = $"Grid Pos: ({x}, {z})";
                gridTile.transform.SetParent(transform);

                grid[x, z] = gridTile;
                // Add the tile to the list
                gridTiles.Add(gridTile); 
            }
        }
    }

    // Get the list of grid tiles and Clear it
    private void EnsureNoDuplicateTiles()
    {
        foreach (var tile in gridTiles)
        {
            if (tile != null)
                DestroyImmediate(tile);
        }

        gridTiles.Clear();
    }

    private void Update()
    {
        RaycastOnMouseClick();
    }

    //raycasting to detect the grid tile that was clicked
    private void RaycastOnMouseClick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out var hit)) 
        {
            var currentTile = hit.collider.GetComponent<GridTile>();
            RaycastProcess(currentTile, currentTile != null);
        }
        else
        {
            RaycastProcess(null);
        }
    }

    // Gets the current tile, sets the highlight effect and the position text
    private void RaycastProcess(GridTile currentTile, bool hit = false)
    {
        highlightEffectGameObject.SetActive(hit);
        if (uiManager == null || !hit) return;
        highlightEffectGameObject.transform.position = new Vector3(currentTile.transform.position.x, highlightPositionY, currentTile.transform.position.z);
        uiManager.SetPositionText(currentTile.GridX, currentTile.GridZ);
    }

    // Method to get neighboring tiles
    public List<GridTile> GetNeighbours(GridTile tile)
    {
        List<GridTile> neighbours = new List<GridTile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int checkX = tile.GridX + x;
                int checkZ = tile.GridZ + z;

                if (checkX >= 0 && checkX < xSize && checkZ >= 0 && checkZ < zSize)
                {
                    neighbours.Add(grid[checkX, checkZ].GetComponent<GridTile>());
                }
            }
        }

        return neighbours;
    }
}