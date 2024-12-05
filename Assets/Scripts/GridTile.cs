using UnityEngine;

//[ExecuteAlways]
public class GridTile : MonoBehaviour
{
    public int GridX { get; private set; }
    public int GridZ { get; private set; }
    private bool _isWalkable = true;
    private bool _hasObstacle;

    public void SetPosition(int x, int z)
    {
        GridX = x;
        GridZ = z;
    }

    public bool IsWalkable 
    { 
        get => _isWalkable && !_hasObstacle;
        set => _isWalkable = value; 
    }
    
    public void SetObstacle(bool hasObstacle)
    {
        _hasObstacle = hasObstacle;
    }
}