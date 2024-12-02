using UnityEditor;
using UnityEngine;

public class FolderCreator : Editor
{
    [MenuItem("Tools/Create Default Folders")]
    private static void CreateDefaultFolders()
    {
        string[] defaultFolders = { "Animations", "Audio", "Materials", "Models", "Prefabs", "Scripts", "Textures" };

        foreach (string folder in defaultFolders)
        {
            string folderPath = $"Assets/{folder}";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", folder);
                Debug.Log($"Created folder: {folderPath}");
            }
            else
            {
                Debug.Log($"Folder already exists in: {folderPath}");
            }
        }

        AssetDatabase.Refresh();
    }
}