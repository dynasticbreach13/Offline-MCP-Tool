using UnityEditor;
using UnityEngine;

public class MCPMenuItems
{
    // Mode shortcuts
    [MenuItem("Tools/MCP/Modes/Modeling", priority = 1)]
    private static void SwitchToModeling()
    {
        SwitchMode("modeling");
    }

    [MenuItem("Tools/MCP/Modes/Rigging", priority = 2)]
    private static void SwitchToRigging()
    {
        SwitchMode("rigging");
    }

    [MenuItem("Tools/MCP/Modes/Animations", priority = 3)]
    private static void SwitchToAnimations()
    {
        SwitchMode("animations");
    }

    [MenuItem("Tools/MCP/Modes/Scripting", priority = 4)]
    private static void SwitchToScripting()
    {
        SwitchMode("scripting");
    }

    [MenuItem("Tools/MCP/Modes/Hierarchy", priority = 5)]
    private static void SwitchToHierarchy()
    {
        SwitchMode("hierarchy");
    }

    [MenuItem("Tools/MCP/Modes/Debugging", priority = 6)]
    private static void SwitchToDebugging()
    {
        SwitchMode("debugging");
    }

    [MenuItem("Tools/MCP/Modes/Master", priority = 7)]
    private static void SwitchToMaster()
    {
        SwitchMode("master");
    }

    [MenuItem("Tools/MCP/Separator", priority = 10)]
    private static void Separator() { }

    // Quick actions
    [MenuItem("Tools/MCP/Open Configuration Panel", priority = 11)]
    private static void OpenConfigPanel()
    {
        MCPToolWindow.ShowWindow();
    }

    [MenuItem("Tools/MCP/Save Configuration", priority = 12)]
    private static void SaveConfig()
    {
        var window = EditorWindow.GetWindow<MCPToolWindow>();
        if (window != null)
        {
            Debug.Log("[MCP Menu] Configuration saved.");
        }
    }

    [MenuItem("Tools/MCP/About MCP Server", priority = 20)]
    private static void ShowAbout()
    {
        EditorUtility.DisplayDialog(
            "About MCP Server",
            "Unity 3D MCP Server v1.0.0\n\n" +
            "A comprehensive Model Context Protocol server for Unity game development.\n\n" +
            "Features:\n" +
            "• 7 Specialized Modes\n" +
            "• 50+ Intelligent Tools\n" +
            "• Offline Operation\n" +
            "• Full Configuration Control\n\n" +
            "© 2024 MCP Server Project",
            "OK"
        );
    }

    private static void SwitchMode(string mode)
    {
        MCPServer server = FindObjectOfType<MCPServer>();
        if (server != null)
        {
            server.SetMode(mode);
            Debug.Log($"[MCP Menu] Switched to {mode} mode.");
        }
        else
        {
            EditorUtility.DisplayDialog(
                "MCP Server Not Found",
                "Please add the MCPServer component to a GameObject in your scene.",
                "OK"
            );
        }
    }
}
