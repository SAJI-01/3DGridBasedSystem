using UnityEngine;

public class GridTile : MonoBehaviour
{
    public int GridX { get; private set; }
    public int GridZ { get; private set; }
    private bool _isWalkable { get; set; } = true;

    public void SetPosition(int x, int z)
    {
        GridX = x;
        GridZ = z;
    }

    private bool HasObstacle { get; set; }
    
    public bool IsWalkable 
    { 
        //Check if the tile is walkable and if there is an not obstacle in way 
        get => _isWalkable && !HasObstacle;
        set => _isWalkable = value; 
    }
    
    public void SetObstacle(bool hasObstacle)
    {
        HasObstacle = hasObstacle;
    }
}