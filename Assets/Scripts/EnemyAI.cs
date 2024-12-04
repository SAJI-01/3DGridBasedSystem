using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour, IAI
{
    [SerializeField] private Animator animator;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PathfindingSystem pathfinding;
    [SerializeField] private GridCreator gridCreator;
    [SerializeField] private GridCharacter gridCharacter;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform playerTransform;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private List<GridTile> currentPath;
    private int currentPathIndex;
    private bool isMoving;
    private Vector3 lastKnownPlayerPosition;
    private GridTile currentOccupiedTile;

    private void Update()
    {
        PathProcess();
        HandleMovement();
        HandleAnimation();
        
    }

    //Get the player's position 
    public void Initialization()
    {
        if (playerTransform == null)
            playerTransform = gridCharacter.transform;
            
        lastKnownPlayerPosition = playerTransform.position;
        UpdateEnemyObstacle();
    }

    //If the player is moving, distance between player and last known position is greater. Find path to player
    public void PathProcess()
    {
        if (!isMoving && !gridCharacter.isMoving && Vector3.Distance(playerTransform.position, lastKnownPlayerPosition) > 0.1f)
        {
            lastKnownPlayerPosition = playerTransform.position;
            FindPathToPlayer();
        }
    }

    private void FindPathToPlayer()
    {
        // Get the player's tile using A* pathfinding system
        var playerTile = pathfinding.GetTileFromGridPosition(GetVector3IntPosition(playerTransform.position));
        if (playerTile == null) return;

        //Getting the adjacent/ neighbouring tiles of the player
        var adjacentTiles = GetAdjacentTiles(playerTile);
        //best Tile is the tile that is closest to the player
        var bestTile = GetBestTileNearPlayer(adjacentTiles);

        if (bestTile != null)
        {
            currentPath = pathfinding.FindPath(GetVector3IntPosition(transform.position), GetVector3IntPosition(bestTile.transform.position));

            if (currentPath != null && currentPath.Count > 0)
            {
                currentPathIndex = 0;
                isMoving = true;
            }
        }
    }

    public void HandleMovement()
    {
        if (gridCharacter.isMoving || !isMoving || currentPath == null || currentPathIndex >= currentPath.Count) return;

        // Move the enemy to the next tile in the path and maintain the y position
        var targetPosition = currentPath[currentPathIndex].transform.position;
        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);
        uiManager.enemyText.transform.rotation = Camera.main.transform.rotation;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            UpdateEnemyObstacle();
            currentPathIndex++;
            isMoving = currentPathIndex < currentPath.Count;
        }
    }
    
    //Play animation for enemy movement
    public void HandleAnimation()
    {
        animator.SetBool(IsMoving, isMoving);
    }
    
    
   //Get the adjacent tiles of the player 
    private List<GridTile> GetAdjacentTiles(GridTile playerTile)
    {
        Vector2Int[] directions = 
            {
                new Vector2Int(0, 1),  // right
                new Vector2Int(1, 0),  // up
                new Vector2Int(0, -1), // left
                new Vector2Int(-1, 0)  // down
            };

        List<GridTile> adjacentTiles = new List<GridTile>();
        
        //Foreach direction, check if the position is valid and if the tile is walkable
        foreach (var dir in directions)
        {
            Vector2Int newPos = new Vector2Int(playerTile.GridX + dir.x, playerTile.GridZ + dir.y);
            if (IsValidPosition(newPos))
            {
                GridTile tile = pathfinding.GetTileFromGridPosition(new Vector3Int(newPos.x, 0, newPos.y));
                if (tile != null && tile.IsWalkable)
                {
                    adjacentTiles.Add(tile);
                }
            }
        }

        return adjacentTiles;
    }
    
    private bool IsValidPosition(Vector2Int position)
    {
        // Check if the position is within the grid boundaries
        return position.x >= 0 && position.x < gridCreator.xSize && position.y >= 0 && position.y < gridCreator.zSize;
    }

    //Get the tile that is closest to the player out of the adjacent tiles direction.
    private GridTile GetBestTileNearPlayer(List<GridTile> adjacentTiles)
    {
        var minDistance = float.MaxValue;
        GridTile bestTile = null;

        foreach (var tile in adjacentTiles)
        {
            var path = pathfinding.FindPath(GetVector3IntPosition(transform.position), GetVector3IntPosition(tile.transform.position));

            if (path != null && path.Count > 0)
            {
                //Get the distance between the enemy and the tile and if it is less than the minimum distance, set the best tile to the current tile
                var distance = Vector3.Distance(transform.position, tile.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestTile = tile;
                }
            }
        }

        return bestTile;
    }
    
    private void UpdateEnemyObstacle()
    {
        Vector3Int currentPos = GetVector3IntPosition(transform.position);
        GridTile newTile = pathfinding.GetTileFromGridPosition(currentPos);
        
        // If we're on a new tile
        if (newTile != currentOccupiedTile)
        {
            // Remove obstacle from previous tile
            if (currentOccupiedTile != null)
            {
                currentOccupiedTile.SetObstacle(false);
            }
            
            // Mark new tile as obstacle
            if (newTile != null)
            {
                newTile.SetObstacle(true);
                currentOccupiedTile = newTile;
            }
        }
    }
    

    //Get only int values of the position , not float
    private Vector3Int GetVector3IntPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y),
            Mathf.RoundToInt(position.z)
        );
    }
    
    private void OnDestroy()
    {
        if (currentOccupiedTile != null)
        {
            currentOccupiedTile.SetObstacle(false);
        }
    }
}