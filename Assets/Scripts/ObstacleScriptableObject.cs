using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Grid/ObstacleData")]
public class ObstacleScriptableObject : ScriptableObject
{
    [Header("Grid Size")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    
    public int XSize => xSize;
    public int ZSize => zSize;

    [SerializeField] private bool[] serializedObstacles;
    [System.NonSerialized] public bool[,] obstacleGrid;

    private void OnEnable()
    {
        InitializeGrid();
        LoadFromSerialized();
    }

    private void OnValidate()
    {
        xSize = Mathf.Max(1, xSize);
        zSize = Mathf.Max(1, zSize);
        InitializeGrid();
        LoadFromSerialized();
    }

    private void InitializeGrid()
    {
        if (obstacleGrid == null || obstacleGrid.GetLength(0) != xSize || obstacleGrid.GetLength(1) != zSize)
        {
            obstacleGrid = new bool[xSize, zSize];
        }

        if (serializedObstacles == null || serializedObstacles.Length != xSize * zSize)
        {
            serializedObstacles = new bool[xSize * zSize];
        }
    }

    public void SaveToSerialized()
    {
        if (obstacleGrid == null) return;
        
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                int index = x * zSize + z;
                if (index < serializedObstacles.Length)
                {
                    serializedObstacles[index] = obstacleGrid[x, z];
                }
            }
        }

        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        #endif
    }

    public void LoadFromSerialized()
    {
        if (serializedObstacles == null || obstacleGrid == null) return;
        
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                int index = x * zSize + z;
                if (index < serializedObstacles.Length)
                {
                    obstacleGrid[x, z] = serializedObstacles[index];
                }
            }
        }
    }
}