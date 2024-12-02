using UnityEngine;

public class GridTile : MonoBehaviour
{
    public int GridX { get; private set; }
    public int GridZ { get; private set; }


    public void SetPosition(int x, int z)
    {
        GridX = x;
        GridZ = z;
    }
}