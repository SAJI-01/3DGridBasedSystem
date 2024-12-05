using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Grid/ObstacleData")]
public class ObstacleScriptableObject : ScriptableObject
{
    [Header("Ensure that X,Z Size Match the ObstacleManager & GridCreator")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    [System.NonSerialized] public bool[,] obstaclesToggles;

    public bool[] serializedToggles;

    private void OnValidate()
    {
        xSize = Mathf.Max(1, xSize);
        zSize = Mathf.Max(1, zSize);

        if (obstaclesToggles == null || obstaclesToggles.GetLength(0) != xSize || obstaclesToggles.GetLength(1) != zSize)
        {
            ResizeArray();
        }

        if (serializedToggles == null || serializedToggles.Length != xSize * zSize)
        {
            serializedToggles = new bool[xSize * zSize];
        }
    }

    private void ResizeArray()
    {
        bool[,] newObstacles = new bool[xSize, zSize];

        if (obstaclesToggles != null)
        {
            int minX = Mathf.Min(obstaclesToggles.GetLength(0), xSize);
            int minZ = Mathf.Min(obstaclesToggles.GetLength(1), zSize);

            for (int x = 0; x < minX; x++)
            {
                for (int z = 0; z < minZ; z++)
                {
                    newObstacles[x, z] = obstaclesToggles[x, z];
                }
            }
        }

        obstaclesToggles = newObstacles;
    }

    public void SaveToSerializedArray()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                serializedToggles[x * zSize + z] = obstaclesToggles[x, z];
            }
        }
    }

    public void LoadFromSerializedArray()
    {
        if (serializedToggles == null || serializedToggles.Length != xSize * zSize)
            return;

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                obstaclesToggles[x, z] = serializedToggles[x * zSize + z];
            }
        }
    }
}
