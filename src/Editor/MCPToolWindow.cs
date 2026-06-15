using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MCPToolWindow : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private string selectedMode = "master";
    private MCPServer mcpServer;
    private GUIStyle modeButtonStyle;
    private GUIStyle panelHeaderStyle;
    private GUIStyle toolItemStyle;
    private bool showConfigPanel = true;
    private bool showToolsPanel = true;
    private bool showLogPanel = false;
    private string logContent = "";

    // Mode configuration
    private static readonly string[] MODES = { "modeling", "rigging", "animations", "scripting", "hierarchy", "debugging", "master" };
    private static readonly string[] MODE_LABELS = { "🎨 Modeling", "🦴 Rigging", "🎬 Animations", "📝 Scripting", "📦 Hierarchy", "🐛 Debugging", "🎮 Master" };
    private static readonly Color[] MODE_COLORS = 
    {
        new Color(1.0f, 0.8f, 0.2f),   // Modeling - Yellow
        new Color(0.8f, 0.6f, 0.4f),   // Rigging - Brown
        new Color(0.2f, 0.7f, 1.0f),   // Animation - Blue
        new Color(0.9f, 0.4f, 0.6f),   // Scripting - Pink
        new Color(0.4f, 0.8f, 0.4f),   // Hierarchy - Green
        new Color(0.9f, 0.3f, 0.3f),   // Debugging - Red
        new Color(1.0f, 0.6f, 0.0f)    // Master - Orange
    };

    // Tool registry per mode
    private Dictionary<string, List<ToolToggle>> modeTools = new Dictionary<string, List<ToolToggle>>();
    private Dictionary<string, bool> toolStates = new Dictionary<string, bool>();

    [MenuItem("Window/MCP Tools/Configuration Panel")]
    public static void ShowWindow()
    {
        var window = GetWindow<MCPToolWindow>("MCP Tools");
        window.minSize = new Vector2(400, 300);
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        InitializeStyles();
        InitializeToolRegistry();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (mcpServer == null)
        {
            mcpServer = FindObjectOfType<MCPServer>();
        }
    }

    private void InitializeStyles()
    {
        modeButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(8, 8, 8, 8),
            margin = new RectOffset(4, 4, 4, 4),
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fixedHeight = 50
        };

        panelHeaderStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(8, 8, 4, 4),
            normal = { textColor = Color.white }
        };

        toolItemStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(8, 8, 6, 6),
            margin = new RectOffset(4, 4, 2, 2),
            normal = { textColor = Color.white }
        };
    }

    private void InitializeToolRegistry()
    {
        // Modeling Mode Tools
        modeTools["modeling"] = new List<ToolToggle>
        {
            new ToolToggle { name = "mesh_analyzer", label = "Mesh Analyzer", enabled = true },
            new ToolToggle { name = "material_helper", label = "Material Helper", enabled = true },
            new ToolToggle { name = "texture_guide", label = "Texture Guide", enabled = true },
            new ToolToggle { name = "lod_generator", label = "LOD Generator", enabled = false },
            new ToolToggle { name = "optimization_checker", label = "Optimization Checker", enabled = true },
            new ToolToggle { name = "uv_analyzer", label = "UV Analyzer", enabled = false },
            new ToolToggle { name = "normal_baker", label = "Normal Baker", enabled = false },
            new ToolToggle { name = "mesh_merger", label = "Mesh Merger", enabled = false },
            new ToolToggle { name = "model_importer", label = "Model Importer", enabled = true }
        };

        // Rigging Mode Tools
        modeTools["rigging"] = new List<ToolToggle>
        {
            new ToolToggle { name = "skeleton_builder", label = "Skeleton Builder", enabled = true },
            new ToolToggle { name = "weight_painter", label = "Weight Painter", enabled = true },
            new ToolToggle { name = "constraint_helper", label = "Constraint Helper", enabled = true },
            new ToolToggle { name = "bone_hierarchy", label = "Bone Hierarchy", enabled = true },
            new ToolToggle { name = "ik_setup", label = "IK Setup", enabled = false },
            new ToolToggle { name = "blend_shape_helper", label = "Blend Shape Helper", enabled = false },
            new ToolToggle { name = "bone_optimizer", label = "Bone Optimizer", enabled = false }
        };

        // Animation Mode Tools
        modeTools["animations"] = new List<ToolToggle>
        {
            new ToolToggle { name = "animation_clipper", label = "Animation Clipper", enabled = true },
            new ToolToggle { name = "state_machine", label = "State Machine", enabled = true },
            new ToolToggle { name = "blend_tree", label = "Blend Tree Helper", enabled = true },
            new ToolToggle { name = "keyframe_editor", label = "Keyframe Editor", enabled = false },
            new ToolToggle { name = "animation_preview", label = "Animation Preview", enabled = true },
            new ToolToggle { name = "transition_helper", label = "Transition Helper", enabled = false },
            new ToolToggle { name = "animation_optimizer", label = "Animation Optimizer", enabled = false },
            new ToolToggle { name = "motion_capture", label = "Motion Capture Helper", enabled = false }
        };

        // Scripting Mode Tools
        modeTools["scripting"] = new List<ToolToggle>
        {
            new ToolToggle { name = "script_generator", label = "Script Generator", enabled = true },
            new ToolToggle { name = "api_reference", label = "API Reference", enabled = true },
            new ToolToggle { name = "code_snippets", label = "Code Snippets", enabled = true },
            new ToolToggle { name = "design_patterns", label = "Design Patterns", enabled = false },
            new ToolToggle { name = "unity_shortcuts", label = "Unity Shortcuts", enabled = true },
            new ToolToggle { name = "serialization_helper", label = "Serialization Helper", enabled = false },
            new ToolToggle { name = "performance_tips", label = "Performance Tips", enabled = true },
            new ToolToggle { name = "code_analyzer", label = "Code Analyzer", enabled = false },
            new ToolToggle { name = "refactoring_guide", label = "Refactoring Guide", enabled = false }
        };

        // Hierarchy Mode Tools
        modeTools["hierarchy"] = new List<ToolToggle>
        {
            new ToolToggle { name = "naming_convention", label = "Naming Convention", enabled = true },
            new ToolToggle { name = "hierarchy_organizer", label = "Hierarchy Organizer", enabled = true },
            new ToolToggle { name = "batch_renamer", label = "Batch Renamer", enabled = true },
            new ToolToggle { name = "folder_structure", label = "Folder Structure", enabled = true },
            new ToolToggle { name = "asset_tagger", label = "Asset Tagger", enabled = false },
            new ToolToggle { name = "scene_cleaner", label = "Scene Cleaner", enabled = true },
            new ToolToggle { name = "duplicate_finder", label = "Duplicate Finder", enabled = false },
            new ToolToggle { name = "prefab_organizer", label = "Prefab Organizer", enabled = false }
        };

        // Debugging Mode Tools
        modeTools["debugging"] = new List<ToolToggle>
        {
            new ToolToggle { name = "error_analyzer", label = "Error Analyzer", enabled = true },
            new ToolToggle { name = "performance_profiler", label = "Performance Profiler", enabled = true },
            new ToolToggle { name = "memory_analyzer", label = "Memory Analyzer", enabled = true },
            new ToolToggle { name = "physics_debugger", label = "Physics Debugger", enabled = false },
            new ToolToggle { name = "ai_debugger", label = "AI Debugger", enabled = false },
            new ToolToggle { name = "network_debugger", label = "Network Debugger", enabled = false },
            new ToolToggle { name = "save_system_tester", label = "Save System Tester", enabled = false },
            new ToolToggle { name = "event_logger", label = "Event Logger", enabled = true },
            new ToolToggle { name = "scene_profiler", label = "Scene Profiler", enabled = false },
            new ToolToggle { name = "frame_debugger", label = "Frame Debugger", enabled = false }
        };

        // Master Mode Tools (all tools)
        var allTools = new List<ToolToggle>();
        foreach (var toolList in modeTools.Values)
        {
            allTools.AddRange(toolList.Select(t => new ToolToggle { name = t.name, label = t.label, enabled = true }));
        }
        modeTools["master"] = allTools.Distinct(new ToolToggleComparer()).ToList();

        // Initialize tool states
        foreach (var mode in modeTools)
        {
            foreach (var tool in mode.Value)
            {
                string key = $"{mode.Key}_{tool.name}";
                if (!toolStates.ContainsKey(key))
                {
                    toolStates[key] = tool.enabled;
                }
            }
        }
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Title
        GUILayout.Space(10);
        GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        DrawTitle();
        GUI.backgroundColor = Color.white;

        // Server Status
        GUILayout.Space(5);
        DrawServerStatus();

        // Mode Selection
        GUILayout.Space(10);
        DrawModeSelection();

        // Tools Panel
        GUILayout.Space(10);
        DrawToolsPanel();

        // Action Buttons
        GUILayout.Space(10);
        DrawActionButtons();

        GUILayout.EndScrollView();
    }

    private void DrawTitle()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("MCP Server Configuration", new GUIStyle(GUI.skin.label) 
        { 
            fontSize = 16, 
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(10, 10, 8, 8)
        });
        GUILayout.Label("Mode Switching & Tool Management", new GUIStyle(GUI.skin.label) 
        { 
            fontSize = 10,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.gray }
        });
        GUILayout.EndVertical();
    }

    private void DrawServerStatus()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        
        if (mcpServer != null)
        {
            GUI.backgroundColor = new Color(0.3f, 0.7f, 0.3f);
            GUILayout.Label("✓ Server Connected", panelHeaderStyle);
            GUI.backgroundColor = Color.white;
            
            GUILayout.Label($"Current Mode: {selectedMode.ToUpper()}", new GUIStyle(GUI.skin.label) { padding = new RectOffset(8, 8, 4, 4) });
            
            int enabledTools = 0;
            if (modeTools.ContainsKey(selectedMode))
            {
                enabledTools = modeTools[selectedMode].Count(t => 
                    toolStates.ContainsKey($"{selectedMode}_{t.name}") && 
                    toolStates[$"{selectedMode}_{t.name}"]
                );
            }
            GUILayout.Label($"Enabled Tools: {enabledTools}", new GUIStyle(GUI.skin.label) { padding = new RectOffset(8, 8, 4, 4) });
        }
        else
        {
            GUI.backgroundColor = new Color(0.7f, 0.3f, 0.3f);
            GUILayout.Label("✗ Server Not Found", panelHeaderStyle);
            GUI.backgroundColor = Color.white;
            GUILayout.Label("Add MCPServer component to a GameObject in the scene.", new GUIStyle(GUI.skin.label) { padding = new RectOffset(8, 8, 4, 4), wordWrap = true });
        }
        
        GUILayout.EndVertical();
    }

    private void DrawModeSelection()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
        GUILayout.Label("SELECT MODE", panelHeaderStyle);
        GUI.backgroundColor = Color.white;

        GUILayout.BeginVertical();
        GUILayout.Space(5);

        for (int i = 0; i < MODES.Length; i += 2)
        {
            GUILayout.BeginHorizontal();

            // First button
            GUI.backgroundColor = selectedMode == MODES[i] ? MODE_COLORS[i] : Color.gray;
            if (GUILayout.Button(MODE_LABELS[i], modeButtonStyle, GUILayout.Height(45)))
            {
                selectedMode = MODES[i];
                if (mcpServer != null)
                {
                    mcpServer.SetMode(selectedMode);
                }
            }
            GUI.backgroundColor = Color.white;

            // Second button (if exists)
            if (i + 1 < MODES.Length)
            {
                GUI.backgroundColor = selectedMode == MODES[i + 1] ? MODE_COLORS[i + 1] : Color.gray;
                if (GUILayout.Button(MODE_LABELS[i + 1], modeButtonStyle, GUILayout.Height(45)))
                {
                    selectedMode = MODES[i + 1];
                    if (mcpServer != null)
                    {
                        mcpServer.SetMode(selectedMode);
                    }
                }
                GUI.backgroundColor = Color.white;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    private void DrawToolsPanel()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        
        // Panel header with toggle
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.2f, 0.25f, 0.2f);
        showToolsPanel = EditorGUILayout.Foldout(showToolsPanel, "TOOLS & TOGGLES", true, new GUIStyle(EditorStyles.foldout) { fontSize = 11, fontStyle = FontStyle.Bold });
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        if (showToolsPanel && modeTools.ContainsKey(selectedMode))
        {
            GUILayout.Space(8);
            var tools = modeTools[selectedMode];

            if (tools.Count == 0)
            {
                GUILayout.Label("No tools available for this mode.", new GUIStyle(GUI.skin.label) { padding = new RectOffset(8, 8, 4, 4) });
            }
            else
            {
                foreach (var tool in tools)
                {
                    DrawToolToggle(tool);
                }
            }
        }

        GUILayout.EndVertical();
    }

    private void DrawToolToggle(ToolToggle tool)
    {
        string key = $"{selectedMode}_{tool.name}";
        bool currentState = toolStates.ContainsKey(key) ? toolStates[key] : tool.enabled;

        GUILayout.BeginHorizontal(toolItemStyle);
        
        bool newState = EditorGUILayout.Toggle(currentState, GUILayout.Width(20));
        if (newState != currentState)
        {
            toolStates[key] = newState;
            if (mcpServer != null)
            {
                mcpServer.ToggleTool(tool.name, newState);
            }
        }

        GUILayout.Label(tool.label, new GUIStyle(GUI.skin.label) { padding = new RectOffset(8, 8, 2, 2), alignment = TextAnchor.MiddleLeft });
        GUILayout.FlexibleSpace();
        
        GUI.backgroundColor = currentState ? new Color(0.4f, 0.7f, 0.4f) : new Color(0.5f, 0.5f, 0.5f);
        GUILayout.Label(currentState ? "ON" : "OFF", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight, fontStyle = FontStyle.Bold }, GUILayout.Width(35));
        GUI.backgroundColor = Color.white;
        
        GUILayout.EndHorizontal();
    }

    private void DrawActionButtons()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = new Color(0.25f, 0.2f, 0.2f);
        GUILayout.Label("ACTIONS", panelHeaderStyle);
        GUI.backgroundColor = Color.white;

        GUILayout.Space(5);

        // Save Config Button
        GUI.backgroundColor = new Color(0.4f, 0.6f, 0.9f);
        if (GUILayout.Button("💾 Save Configuration", GUILayout.Height(35)))
        {
            SaveConfiguration();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(3);

        // Load Config Button
        GUI.backgroundColor = new Color(0.9f, 0.7f, 0.4f);
        if (GUILayout.Button("📂 Load Configuration", GUILayout.Height(35)))
        {
            LoadConfiguration();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(3);

        // Reset to Defaults
        GUI.backgroundColor = new Color(0.9f, 0.4f, 0.4f);
        if (GUILayout.Button("🔄 Reset to Defaults", GUILayout.Height(35)))
        {
            ResetToDefaults();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(5);
        GUILayout.EndVertical();
    }

    private void SaveConfiguration()
    {
        var config = new ConfigData
        {
            defaultMode = selectedMode,
            server = new ServerInfo { name = "Unity3D-MCP-Server", version = "1.0.0" },
            modes = new Dictionary<string, ModeConfig>()
        };

        // Build mode configurations
        foreach (var mode in MODES)
        {
            if (modeTools.ContainsKey(mode))
            {
                var modeConfig = new ModeConfig { tools = new Dictionary<string, ToolConfig>() };
                
                foreach (var tool in modeTools[mode])
                {
                    string key = $"{mode}_{tool.name}";
                    bool enabled = toolStates.ContainsKey(key) ? toolStates[key] : tool.enabled;
                    modeConfig.tools[tool.name] = new ToolConfig { enabled = enabled };
                }

                config.modes[mode] = modeConfig;
            }
        }

        // Save to JSON
        string json = JsonUtility.ToJson(config, true);
        string path = "Assets/Resources/config.json";
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllText(path, json);
        
        EditorUtility.DisplayDialog("Success", "Configuration saved to Assets/Resources/config.json", "OK");
        Debug.Log("[MCP Config] Configuration saved successfully.");
    }

    private void LoadConfiguration()
    {
        string path = "Assets/Resources/config.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            var config = JsonUtility.FromJson<ConfigData>(json);
            
            if (config != null)
            {
                selectedMode = config.defaultMode;
                
                // Load tool states from config
                foreach (var mode in config.modes)
                {
                    foreach (var tool in mode.Value.tools)
                    {
                        string key = $"{mode.Key}_{tool.Key}";
                        toolStates[key] = tool.Value.enabled;
                    }
                }

                EditorUtility.DisplayDialog("Success", "Configuration loaded successfully.", "OK");
                Debug.Log("[MCP Config] Configuration loaded from Assets/Resources/config.json");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Configuration file not found at Assets/Resources/config.json", "OK");
        }
    }

    private void ResetToDefaults()
    {
        if (EditorUtility.DisplayDialog("Reset Configuration", "Are you sure you want to reset all tools to default states?", "Yes", "No"))
        {
            // Reset tool states
            toolStates.Clear();
            InitializeToolRegistry();
            selectedMode = "master";
            
            if (mcpServer != null)
            {
                mcpServer.SetMode("master");
            }
            
            Debug.Log("[MCP Config] Configuration reset to defaults.");
        }
    }
}

[System.Serializable]
public class ConfigData
{
    public string defaultMode;
    public ServerInfo server;
    public Dictionary<string, ModeConfig> modes;
}

[System.Serializable]
public class ServerInfo
{
    public string name;
    public string version;
}

[System.Serializable]
public class ModeConfig
{
    public Dictionary<string, ToolConfig> tools;
}

[System.Serializable]
public class ToolConfig
{
    public bool enabled;
}

public class ToolToggle
{
    public string name;
    public string label;
    public bool enabled;
}

public class ToolToggleComparer : IEqualityComparer<ToolToggle>
{
    public bool Equals(ToolToggle x, ToolToggle y)
    {
        return x?.name == y?.name;
    }

    public int GetHashCode(ToolToggle obj)
    {
        return obj?.name?.GetHashCode() ?? 0;
    }
}