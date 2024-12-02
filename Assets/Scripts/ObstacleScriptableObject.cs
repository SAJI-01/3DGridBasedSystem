using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Grid/ObstacleData")]
public class ObstacleScriptableObject : ScriptableObject
{
    [Header("Ensure that X,Z Size Match the ObstacleManager & GridCreator")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    public bool[,] Obstacles;

    private void OnValidate()
    {
        xSize = Mathf.Max(1, xSize);
        zSize = Mathf.Max(1, zSize);
        
        if (Obstacles == null || Obstacles.GetLength(0) != xSize || Obstacles.GetLength(1) != zSize)
        {
            bool[,] newObstacles = new bool[xSize, zSize];
            
            if (Obstacles != null)
            {
                int minX = Mathf.Min(Obstacles.GetLength(0), xSize);
                int minZ = Mathf.Min(Obstacles.GetLength(1), zSize);
                
                for (int x = 0; x < minX; x++)
                    for (int z = 0; z < minZ; z++)
                        newObstacles[x, z] = Obstacles[x, z];
            }
            
            Obstacles = newObstacles;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObstacleScriptableObject))]
public class ObstacleDataEditor : Editor
{
    private ObstacleManager _obstacleManager;

    private void OnEnable() => _obstacleManager = FindObjectOfType<ObstacleManager>();

    public override void OnInspectorGUI()
    {
        var data = (ObstacleScriptableObject)target;
        serializedObject.Update();
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("xSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zSize"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Obstacles", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        for (int x = 0; x < data.Obstacles.GetLength(0); x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int z = 0; z < data.Obstacles.GetLength(1); z++)
                data.Obstacles[x, z] = EditorGUILayout.Toggle(data.Obstacles[x, z], GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
            _obstacleManager?.GenerateObstacles();
        }
    }
}
#endif