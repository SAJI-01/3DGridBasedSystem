#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleScriptableObject))]
public class ObstacleDataEditor : Editor
{
    // Reference to manager responsible for obstacle generation
    private ObstacleManager obstacleManager;
    private SerializedProperty xSize;
    private SerializedProperty zSize;

    private void OnEnable()
    {
        xSize = serializedObject.FindProperty("xSize");
        zSize = serializedObject.FindProperty("zSize");
        
        // Getting reference of obstacle manager 
        obstacleManager = FindObjectOfType<ObstacleManager>();
    }

    public override void OnInspectorGUI()
    {
        
        var obstacleScriptableObject = (ObstacleScriptableObject)target;
        serializedObject.Update();
        
        // Handle changes in the size properties
        DrawSizeProperties();
        
        // Draw obstacle grid toggleable UI
        DrawObstacleGrid(obstacleScriptableObject);

        //if changes detected
        HandleChanges(obstacleScriptableObject);
    }

    // Checks for changes in the size properties and apply them
    private void DrawSizeProperties()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(xSize);
        EditorGUILayout.PropertyField(zSize);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }

    // Draw the grid of toggleable obstacles
    private void DrawObstacleGrid(ObstacleScriptableObject data)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Obstacles", EditorStyles.boldLabel);

        // Draw grid of toggleable obstacles vertically and horizontally
        EditorGUILayout.BeginVertical();
        for (int x = 0; x < data.obstaclesToggles.GetLength(0); x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int z = 0; z < data.obstaclesToggles.GetLength(1); z++)
            {
                data.obstaclesToggles[x, z] = EditorGUILayout.Toggle(data.obstaclesToggles[x, z], GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    // When changes are detected, set the data as dirty/modified and generate obstacles
    private void HandleChanges(ObstacleScriptableObject data)
    {
        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);  
            obstacleManager?.GenerateObstacles();
        }
    }
}
#endif