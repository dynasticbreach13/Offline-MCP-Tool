using UnityEngine;
using UnityEditor;

public static class EditorUIStyles
{
    private static GUIStyle headerStyle;
    private static GUIStyle panelStyle;
    private static GUIStyle buttonStyle;
    private static GUIStyle toolItemStyle;

    public static GUIStyle GetHeaderStyle()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 8, 8),
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
        }
        return headerStyle;
    }

    public static GUIStyle GetPanelStyle()
    {
        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(4, 4, 4, 4)
            };
        }
        return panelStyle;
    }

    public static GUIStyle GetButtonStyle()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(8, 8, 6, 6),
                margin = new RectOffset(4, 4, 4, 4),
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
        }
        return buttonStyle;
    }

    public static GUIStyle GetToolItemStyle()
    {
        if (toolItemStyle == null)
        {
            toolItemStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(8, 8, 6, 6),
                margin = new RectOffset(4, 4, 2, 2),
                alignment = TextAnchor.MiddleLeft
            };
        }
        return toolItemStyle;
    }

    public static Color GetModeColor(string mode)
    {
        return mode switch
        {
            "modeling" => new Color(1.0f, 0.8f, 0.2f),
            "rigging" => new Color(0.8f, 0.6f, 0.4f),
            "animations" => new Color(0.2f, 0.7f, 1.0f),
            "scripting" => new Color(0.9f, 0.4f, 0.6f),
            "hierarchy" => new Color(0.4f, 0.8f, 0.4f),
            "debugging" => new Color(0.9f, 0.3f, 0.3f),
            "master" => new Color(1.0f, 0.6f, 0.0f),
            _ => Color.gray
        };
    }

    public static string GetModeEmoji(string mode)
    {
        return mode switch
        {
            "modeling" => "🎨",
            "rigging" => "🦴",
            "animations" => "🎬",
            "scripting" => "📝",
            "hierarchy" => "📦",
            "debugging" => "🐛",
            "master" => "🎮",
            _ => "❓"
        };
    }
}
