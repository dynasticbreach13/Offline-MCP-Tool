using UnityEditor;
using UnityEngine;
using System.IO;

public class MCPConfigImporter
{
    [MenuItem("Tools/MCP/Import Config from File")]
    private static void ImportConfigFromFile()
    {
        string path = EditorUtility.OpenFilePanel(
            "Select MCP Configuration File",
            Application.persistentDataPath,
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                string destinationPath = "Assets/Resources/config.json";
                
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                File.WriteAllText(destinationPath, json);
                
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", "Configuration imported successfully!", "OK");
                Debug.Log($"[MCP Config] Imported configuration from: {path}");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to import configuration: {e.Message}", "OK");
                Debug.LogError($"[MCP Config] Import failed: {e}");
            }
        }
    }

    [MenuItem("Tools/MCP/Export Config to File")]
    private static void ExportConfigToFile()
    {
        string sourcePath = "Assets/Resources/config.json";
        
        if (!File.Exists(sourcePath))
        {
            EditorUtility.DisplayDialog("Error", "No configuration file found at Assets/Resources/config.json", "OK");
            return;
        }

        string path = EditorUtility.SaveFilePanel(
            "Save MCP Configuration",
            Application.persistentDataPath,
            "config",
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string json = File.ReadAllText(sourcePath);
                File.WriteAllText(path, json);
                
                EditorUtility.DisplayDialog("Success", "Configuration exported successfully!", "OK");
                Debug.Log($"[MCP Config] Exported configuration to: {path}");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to export configuration: {e.Message}", "OK");
                Debug.LogError($"[MCP Config] Export failed: {e}");
            }
        }
    }
}
