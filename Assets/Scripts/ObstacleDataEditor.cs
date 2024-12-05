#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleScriptableObject))]
public class ObstacleDataEditor : Editor
{
    private ObstacleManager obstacleManager;
    private SerializedProperty xSize;
    private SerializedProperty zSize;

    private void OnEnable()
    {
        xSize = serializedObject.FindProperty("xSize");
        zSize = serializedObject.FindProperty("zSize");
        obstacleManager = FindObjectOfType<ObstacleManager>();
    }

    public override void OnInspectorGUI()
    {
        var obstacleData = (ObstacleScriptableObject)target;
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        
        // Draw size properties
        EditorGUILayout.PropertyField(xSize);
        EditorGUILayout.PropertyField(zSize);
        
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Obstacle Grid", EditorStyles.boldLabel);

        // Draw obstacle grid
        if (obstacleData.obstacleGrid != null)
        {
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginVertical();
            for (int x = 0; x < obstacleData.XSize; x++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int z = 0; z < obstacleData.ZSize; z++)
                {
                    obstacleData.obstacleGrid[x, z] = EditorGUILayout.Toggle(
                        obstacleData.obstacleGrid[x, z], 
                        GUILayout.Width(20)
                    );
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                obstacleData.SaveToSerialized();
                obstacleManager?.RegenerateObstacles();
            }
        }
    }
}
#endif