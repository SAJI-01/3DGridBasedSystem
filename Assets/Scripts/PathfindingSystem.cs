using System.Collections.Generic;
using UnityEngine;



public class PathfindingSystem : MonoBehaviour
{
    [SerializeField] private GridCreator gridCreator;

    // Find the path from the start position to the target position
    public List<GridTile> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        var startTile = GetTileFromGridPosition(startPos);
        var targetTile = GetTileFromGridPosition(targetPos);

        if (startTile == null || targetTile == null || !targetTile.IsWalkable)
            return null;

        
        var openSet = new List<GridTile>();
        var closedSet = new HashSet<GridTile>();
        var nodeData = new Dictionary<GridTile, PathNode>();

        // Add the start tile to the open set
        openSet.Add(startTile);
        nodeData[startTile] = new PathNode { gCost = 0, hCost = GetDistance(startTile, targetTile) };

        // Loop through the open set until it is empty
        while (openSet.Count > 0)
        {
            var currentTile = GetLowestFCostTile(openSet, nodeData);

            if (currentTile == targetTile)
                return RetracePath(nodeData, startTile, targetTile);

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Loop through the neighbors of the current tile
            foreach (var neighbor in gridCreator.GetNeighbours(currentTile))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                    continue;

                var newGCost = nodeData[currentTile].gCost + GetDistance(currentTile, neighbor);

                // If the neighbor is not in the open set, add it
                if (!nodeData.ContainsKey(neighbor))
                    nodeData[neighbor] = new PathNode();

                // Update the gCost, hCost, and parent tile if the new gCost is less than the current gCost
                if (newGCost < nodeData[neighbor].gCost || !openSet.Contains(neighbor))
                {
                    nodeData[neighbor].gCost = newGCost;
                    nodeData[neighbor].hCost = GetDistance(neighbor, targetTile);
                    nodeData[neighbor].parent = currentTile;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }


    // Get the tile with the lowest F cost from the open set
    private GridTile GetLowestFCostTile(List<GridTile> openSet, Dictionary<GridTile, PathNode> nodeData)
    {
        var lowestTile = openSet[0];
        var lowestFCost = nodeData[lowestTile].FCost;

        for (var i = 1; i < openSet.Count; i++)
        {
            // Compare the F cost of the current tile with the lowest F cost
            var fCost = nodeData[openSet[i]].FCost;
            if (fCost < lowestFCost ||
                (fCost == lowestFCost && nodeData[openSet[i]].hCost < nodeData[lowestTile].hCost))
            {
                lowestTile = openSet[i];
                lowestFCost = fCost;
            }
        }

        return lowestTile;
    }

    // Retrace the path from the end tile to the start tile
    private List<GridTile> RetracePath(Dictionary<GridTile, PathNode> nodeData, GridTile startTile, GridTile endTile)
    {
        var path = new List<GridTile>();
        var currentTile = endTile;

        // Loop through the parent tiles until the start tile is reached
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = nodeData[currentTile].parent;
        }

        path.Reverse();
        return path;
    }

    // Get the distance between two tiles
    private int GetDistance(GridTile tileA, GridTile tileB)
    {
        var distX = Mathf.Abs(tileA.GridX - tileB.GridX);
        var distZ = Mathf.Abs(tileA.GridZ - tileB.GridZ);

        // Calculate the distance using the diagonal distance formula ~ taken it from site: https://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
        return distX > distZ ? 14 * distZ + 10 * (distX - distZ) : 14 * distX + 10 * (distZ - distX);
    }

    // Get the tile from the grid position
    public GridTile GetTileFromGridPosition(Vector3Int gridPosition)
    {
        var x = gridPosition.x;
        var z = gridPosition.z;

        if (x >= 0 && x < gridCreator.xSize && z >= 0 && z < gridCreator.zSize)
            return gridCreator.grid[x, z].GetComponent<GridTile>();
        return null;
    }

    //PathNode class to store the gCost, hCost, and parent tile
    private class PathNode
    {
        public int gCost = int.MaxValue;
        public int hCost;
        public int FCost => gCost + hCost;
        public GridTile parent;
    }
}