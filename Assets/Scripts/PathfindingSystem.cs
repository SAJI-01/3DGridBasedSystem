using System.Collections.Generic;
using UnityEngine;

public class PathfindingSystem : MonoBehaviour
{
    [SerializeField] private GridCreator gridCreator;

    public List<GridTile> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        var startTile = GetTileFromGridPosition(startPos);
        var targetTile = GetTileFromGridPosition(targetPos);

        if (startTile == null || targetTile == null || !targetTile.IsWalkable)
            return null;

        var openSet = new List<GridTile>();
        var closedSet = new HashSet<GridTile>();
        var nodeData = new Dictionary<GridTile, PathNode>();

        openSet.Add(startTile);
        nodeData[startTile] = new PathNode { gCost = 0, hCost = GetDistance(startTile, targetTile) };

        while (openSet.Count > 0)
        {
            var currentTile = GetLowestFCostTile(openSet, nodeData);

            if (currentTile == targetTile)
                return RetracePath(nodeData, startTile, targetTile);

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (var neighbor in gridCreator.GetNeighbours(currentTile))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                    continue;

                var newGCost = nodeData[currentTile].gCost + GetDistance(currentTile, neighbor);

                if (!nodeData.ContainsKey(neighbor))
                    nodeData[neighbor] = new PathNode();

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


    private GridTile GetLowestFCostTile(List<GridTile> openSet, Dictionary<GridTile, PathNode> nodeData)
    {
        var lowestTile = openSet[0];
        var lowestFCost = nodeData[lowestTile].FCost;

        for (var i = 1; i < openSet.Count; i++)
        {
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

    private List<GridTile> RetracePath(Dictionary<GridTile, PathNode> nodeData, GridTile startTile, GridTile endTile)
    {
        var path = new List<GridTile>();
        var currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = nodeData[currentTile].parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(GridTile tileA, GridTile tileB)
    {
        var distX = Mathf.Abs(tileA.GridX - tileB.GridX);
        var distZ = Mathf.Abs(tileA.GridZ - tileB.GridZ);

        return distX > distZ ? 14 * distZ + 10 * (distX - distZ) : 14 * distX + 10 * (distZ - distX);
    }

    public GridTile GetTileFromGridPosition(Vector3Int gridPosition)
    {
        var x = gridPosition.x;
        var z = gridPosition.z;

        if (x >= 0 && x < gridCreator.xSize && z >= 0 && z < gridCreator.zSize)
            return gridCreator.grid[x, z].GetComponent<GridTile>();
        return null;
    }

    private class PathNode
    {
        public int gCost = int.MaxValue;
        public int hCost;
        public int FCost => gCost + hCost;
        public GridTile parent;
    }
}