using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Grid/ObstacleData")]
public class ObstacleScriptableObject : ScriptableObject
{
    [Header("Ensure that X,Z Size Match the ObstacleManager & GridCreator")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    public bool[,] obstaclesToggles;


    private void OnValidate()
    {
        xSize = Mathf.Max(1, xSize);
        zSize = Mathf.Max(1, zSize);
        
        if (obstaclesToggles == null || obstaclesToggles.GetLength(0) != xSize || obstaclesToggles.GetLength(1) != zSize)
        {
            bool[,] newObstacles = new bool[xSize, zSize];
            
            if (obstaclesToggles != null)
            {
                int minX = Mathf.Min(obstaclesToggles.GetLength(0), xSize);
                int minZ = Mathf.Min(obstaclesToggles.GetLength(1), zSize);
                
                for (int x = 0; x < minX; x++)
                    for (int z = 0; z < minZ; z++)
                        newObstacles[x, z] = obstaclesToggles[x, z];
            }
            
            obstaclesToggles = newObstacles;
        }
    }
}