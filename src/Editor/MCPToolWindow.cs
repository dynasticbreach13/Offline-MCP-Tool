using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MCPToolWindow : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 toolScrollPosition = Vector2.zero;
    private string selectedMode = "master";
    private MCPServer mcpServer;
    private GUIStyle modeButtonStyle;
    private GUIStyle panelHeaderStyle;
    private GUIStyle toolItemStyle;
    private bool showConfigPanel = true;
    private bool showToolsPanel = true;
    private bool showPresetPanel = true;
    private bool showLogPanel = false;
    private string logContent = "";
    private string toolSearchFilter = "";
    private string selectedPreset = "custom";

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
    private static readonly string[] MODE_DESCRIPTIONS = 
    {
        "Focus on 3D assets, meshes, materials, and optimization",
        "Focus on skeleton setup, bone hierarchy, and weight painting",
        "Focus on animation clips, state machines, and blending",
        "Focus on C# generation, API references, and best practices",
        "Focus on project organization, naming conventions, and hierarchy",
        "Focus on error analysis, performance profiling, and debugging",
        "Full access to all 50+ tools across all specializations"
    };

    // Tool registry per mode
    private Dictionary<string, List<ToolToggle>> modeTools = new Dictionary<string, List<ToolToggle>>();
    private Dictionary<string, bool> toolStates = new Dictionary<string, bool>();
    private Dictionary<string, ToolPreset> presets = new Dictionary<string, ToolPreset>();

    [MenuItem("Window/MCP Tools/Configuration Panel")]
    public static void ShowWindow()
    {
        var window = GetWindow<MCPToolWindow>("MCP Tools");
        window.minSize = new Vector2(500, 400);
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        InitializeStyles();
        InitializeToolRegistry();
        InitializePresets();
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
        // Modeling Mode Tools with descriptions
        modeTools["modeling"] = new List<ToolToggle>
        {
            new ToolToggle { name = "mesh_analyzer", label = "Mesh Analyzer", enabled = true, description = "Analyze mesh complexity, vertex count, and optimization opportunities" },
            new ToolToggle { name = "material_helper", label = "Material Helper", enabled = true, description = "Create and manage materials with proper shader assignments" },
            new ToolToggle { name = "texture_guide", label = "Texture Guide", enabled = true, description = "Guide for texture mapping, resolution, and format selection" },
            new ToolToggle { name = "lod_generator", label = "LOD Generator", enabled = false, description = "Automatically generate LOD (Level of Detail) meshes" },
            new ToolToggle { name = "optimization_checker", label = "Optimization Checker", enabled = true, description = "Check for common modeling optimization issues" },
            new ToolToggle { name = "uv_analyzer", label = "UV Analyzer", enabled = false, description = "Analyze UV mapping, overlaps, and efficiency" },
            new ToolToggle { name = "normal_baker", label = "Normal Baker", enabled = false, description = "Bake normal maps from high-poly to low-poly models" },
            new ToolToggle { name = "mesh_merger", label = "Mesh Merger", enabled = false, description = "Merge multiple meshes for performance optimization" },
            new ToolToggle { name = "model_importer", label = "Model Importer", enabled = true, description = "Configure and optimize imported 3D models" }
        };

        // Rigging Mode Tools with descriptions
        modeTools["rigging"] = new List<ToolToggle>
        {
            new ToolToggle { name = "skeleton_builder", label = "Skeleton Builder", enabled = true, description = "Create and configure character skeletons and bone hierarchies" },
            new ToolToggle { name = "weight_painter", label = "Weight Painter", enabled = true, description = "Paint skin weights for smooth character deformation" },
            new ToolToggle { name = "constraint_helper", label = "Constraint Helper", enabled = true, description = "Set up bone constraints (parent, look-at, etc.)" },
            new ToolToggle { name = "bone_hierarchy", label = "Bone Hierarchy", enabled = true, description = "Organize and validate bone hierarchy structure" },
            new ToolToggle { name = "ik_setup", label = "IK Setup", enabled = false, description = "Configure Inverse Kinematics for limbs" },
            new ToolToggle { name = "blend_shape_helper", label = "Blend Shape Helper", enabled = false, description = "Create and manage blend shapes for facial animation" },
            new ToolToggle { name = "bone_optimizer", label = "Bone Optimizer", enabled = false, description = "Remove unnecessary bones and optimize hierarchy" }
        };

        // Animation Mode Tools with descriptions
        modeTools["animations"] = new List<ToolToggle>
        {
            new ToolToggle { name = "animation_clipper", label = "Animation Clipper", enabled = true, description = "Extract and organize animation clips from imported FBX files" },
            new ToolToggle { name = "state_machine", label = "State Machine", enabled = true, description = "Create and configure Animator state machines" },
            new ToolToggle { name = "blend_tree", label = "Blend Tree Helper", enabled = true, description = "Set up blend trees for smooth animation transitions" },
            new ToolToggle { name = "keyframe_editor", label = "Keyframe Editor", enabled = false, description = "Edit animation keyframes and curves" },
            new ToolToggle { name = "animation_preview", label = "Animation Preview", enabled = true, description = "Preview animations in real-time with playback controls" },
            new ToolToggle { name = "transition_helper", label = "Transition Helper", enabled = false, description = "Configure smooth transitions between animation states" },
            new ToolToggle { name = "animation_optimizer", label = "Animation Optimizer", enabled = false, description = "Optimize animation file sizes and frame rates" },
            new ToolToggle { name = "motion_capture", label = "Motion Capture Helper", enabled = false, description = "Process and refine motion capture data" }
        };

        // Scripting Mode Tools with descriptions
        modeTools["scripting"] = new List<ToolToggle>
        {
            new ToolToggle { name = "script_generator", label = "Script Generator", enabled = true, description = "Generate C# script templates for common game development patterns" },
            new ToolToggle { name = "api_reference", label = "API Reference", enabled = true, description = "Quick access to Unity API documentation and examples" },
            new ToolToggle { name = "code_snippets", label = "Code Snippets", enabled = true, description = "Reusable code snippets for common tasks" },
            new ToolToggle { name = "design_patterns", label = "Design Patterns", enabled = false, description = "Implement common design patterns (Singleton, Observer, etc.)" },
            new ToolToggle { name = "unity_shortcuts", label = "Unity Shortcuts", enabled = true, description = "Learn and use Unity Editor keyboard shortcuts" },
            new ToolToggle { name = "serialization_helper", label = "Serialization Helper", enabled = false, description = "Configure serialization for custom classes" },
            new ToolToggle { name = "performance_tips", label = "Performance Tips", enabled = true, description = "Best practices and optimization tips for C# code" },
            new ToolToggle { name = "code_analyzer", label = "Code Analyzer", enabled = false, description = "Analyze code for performance and correctness issues" },
            new ToolToggle { name = "refactoring_guide", label = "Refactoring Guide", enabled = false, description = "Guide for safe code refactoring" }
        };

        // Hierarchy Mode Tools with descriptions
        modeTools["hierarchy"] = new List<ToolToggle>
        {
            new ToolToggle { name = "naming_convention", label = "Naming Convention", enabled = true, description = "Apply consistent naming conventions to objects and assets" },
            new ToolToggle { name = "hierarchy_organizer", label = "Hierarchy Organizer", enabled = true, description = "Organize scene hierarchy with folders and groups" },
            new ToolToggle { name = "batch_renamer", label = "Batch Renamer", enabled = true, description = "Rename multiple objects at once with patterns" },
            new ToolToggle { name = "folder_structure", label = "Folder Structure", enabled = true, description = "Create and maintain consistent project folder structure" },
            new ToolToggle { name = "asset_tagger", label = "Asset Tagger", enabled = false, description = "Tag and categorize assets for easy management" },
            new ToolToggle { name = "scene_cleaner", label = "Scene Cleaner", enabled = true, description = "Remove unused objects, assets, and dependencies" },
            new ToolToggle { name = "duplicate_finder", label = "Duplicate Finder", enabled = false, description = "Find and remove duplicate assets and prefabs" },
            new ToolToggle { name = "prefab_organizer", label = "Prefab Organizer", enabled = false, description = "Organize and manage prefab library" }
        };

        // Debugging Mode Tools with descriptions
        modeTools["debugging"] = new List<ToolToggle>
        {
            new ToolToggle { name = "error_analyzer", label = "Error Analyzer", enabled = true, description = "Analyze and suggest fixes for common console errors" },
            new ToolToggle { name = "performance_profiler", label = "Performance Profiler", enabled = true, description = "Profile CPU, GPU, and memory performance" },
            new ToolToggle { name = "memory_analyzer", label = "Memory Analyzer", enabled = true, description = "Identify memory leaks and inefficient allocations" },
            new ToolToggle { name = "physics_debugger", label = "Physics Debugger", enabled = false, description = "Visualize and debug physics collisions" },
            new ToolToggle { name = "ai_debugger", label = "AI Debugger", enabled = false, description = "Debug AI behavior and pathfinding" },
            new ToolToggle { name = "network_debugger", label = "Network Debugger", enabled = false, description = "Monitor and debug network traffic" },
            new ToolToggle { name = "save_system_tester", label = "Save System Tester", enabled = false, description = "Test and validate save/load functionality" },
            new ToolToggle { name = "event_logger", label = "Event Logger", enabled = true, description = "Log and monitor game events for debugging" },
            new ToolToggle { name = "scene_profiler", label = "Scene Profiler", enabled = false, description = "Profile scene load times and asset usage" },
            new ToolToggle { name = "frame_debugger", label = "Frame Debugger", enabled = false, description = "Debug rendering pipeline frame-by-frame" }
        };

        // Master Mode Tools (all tools)
        var allTools = new List<ToolToggle>();
        foreach (var toolList in modeTools.Values)
        {
            allTools.AddRange(toolList.Select(t => new ToolToggle { name = t.name, label = t.label, enabled = true, description = t.description }));
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

    private void InitializePresets()
    {
        // Beginner Preset - Limited tools, safe defaults
        presets["beginner"] = new ToolPreset
        {
            name = "Beginner",
            description = "Limited tools, safe defaults for learning",
            defaultMode = "master",
            toolConfigs = new Dictionary<string, Dictionary<string, bool>>
            {
                { "modeling", new Dictionary<string, bool> { { "mesh_analyzer", true }, { "material_helper", true }, { "texture_guide", true }, { "optimization_checker", true }, { "model_importer", true } } },
                { "rigging", new Dictionary<string, bool> { { "skeleton_builder", true }, { "weight_painter", true }, { "constraint_helper", true }, { "bone_hierarchy", true } } },
                { "animations", new Dictionary<string, bool> { { "animation_clipper", true }, { "state_machine", true }, { "animation_preview", true } } },
                { "scripting", new Dictionary<string, bool> { { "script_generator", true }, { "api_reference", true }, { "code_snippets", true }, { "unity_shortcuts", true } } },
                { "hierarchy", new Dictionary<string, bool> { { "naming_convention", true }, { "hierarchy_organizer", true }, { "batch_renamer", true }, { "folder_structure", true }, { "scene_cleaner", true } } },
                { "debugging", new Dictionary<string, bool> { { "error_analyzer", true }, { "performance_profiler", true }, { "memory_analyzer", true }, { "event_logger", true } } },
            }
        };

        // Artist Preset - Modeling, Rigging, Animation focus
        presets["artist"] = new ToolPreset
        {
            name = "Artist",
            description = "Modeling, Rigging, and Animation tools",
            defaultMode = "modeling",
            toolConfigs = new Dictionary<string, Dictionary<string, bool>>
            {
                { "modeling", new Dictionary<string, bool> { { "mesh_analyzer", true }, { "material_helper", true }, { "texture_guide", true }, { "lod_generator", true }, { "optimization_checker", true }, { "uv_analyzer", true }, { "normal_baker", true }, { "mesh_merger", true }, { "model_importer", true } } },
                { "rigging", new Dictionary<string, bool> { { "skeleton_builder", true }, { "weight_painter", true }, { "constraint_helper", true }, { "bone_hierarchy", true }, { "ik_setup", true }, { "blend_shape_helper", true }, { "bone_optimizer", true } } },
                { "animations", new Dictionary<string, bool> { { "animation_clipper", true }, { "state_machine", true }, { "blend_tree", true }, { "animation_preview", true }, { "transition_helper", true }, { "animation_optimizer", true }, { "motion_capture", true } } },
            }
        };

        // Programmer Preset - Scripting and Debugging focus
        presets["programmer"] = new ToolPreset
        {
            name = "Programmer",
            description = "Scripting, Debugging, and Code tools",
            defaultMode = "scripting",
            toolConfigs = new Dictionary<string, Dictionary<string, bool>>
            {
                { "scripting", new Dictionary<string, bool> { { "script_generator", true }, { "api_reference", true }, { "code_snippets", true }, { "design_patterns", true }, { "unity_shortcuts", true }, { "serialization_helper", true }, { "performance_tips", true }, { "code_analyzer", true }, { "refactoring_guide", true } } },
                { "debugging", new Dictionary<string, bool> { { "error_analyzer", true }, { "performance_profiler", true }, { "memory_analyzer", true }, { "physics_debugger", true }, { "ai_debugger", true }, { "network_debugger", true }, { "save_system_tester", true }, { "event_logger", true }, { "scene_profiler", true }, { "frame_debugger", true } } },
                { "hierarchy", new Dictionary<string, bool> { { "naming_convention", true }, { "hierarchy_organizer", true }, { "batch_renamer", true }, { "folder_structure", true } } },
            }
        };

        // Full Toolkit Preset - Everything enabled
        presets["full"] = new ToolPreset
        {
            name = "Full Toolkit",
            description = "All 50+ tools enabled for comprehensive access",
            defaultMode = "master",
            toolConfigs = new Dictionary<string, Dictionary<string, bool>>
            {
                { "modeling", new Dictionary<string, bool> { { "mesh_analyzer", true }, { "material_helper", true }, { "texture_guide", true }, { "lod_generator", true }, { "optimization_checker", true }, { "uv_analyzer", true }, { "normal_baker", true }, { "mesh_merger", true }, { "model_importer", true } } },
                { "rigging", new Dictionary<string, bool> { { "skeleton_builder", true }, { "weight_painter", true }, { "constraint_helper", true }, { "bone_hierarchy", true }, { "ik_setup", true }, { "blend_shape_helper", true }, { "bone_optimizer", true } } },
                { "animations", new Dictionary<string, bool> { { "animation_clipper", true }, { "state_machine", true }, { "blend_tree", true }, { "keyframe_editor", true }, { "animation_preview", true }, { "transition_helper", true }, { "animation_optimizer", true }, { "motion_capture", true } } },
                { "scripting", new Dictionary<string, bool> { { "script_generator", true }, { "api_reference", true }, { "code_snippets", true }, { "design_patterns", true }, { "unity_shortcuts", true }, { "serialization_helper", true }, { "performance_tips", true }, { "code_analyzer", true }, { "refactoring_guide", true } } },
                { "hierarchy", new Dictionary<string, bool> { { "naming_convention", true }, { "hierarchy_organizer", true }, { "batch_renamer", true }, { "folder_structure", true }, { "asset_tagger", true }, { "scene_cleaner", true }, { "duplicate_finder", true }, { "prefab_organizer", true } } },
                { "debugging", new Dictionary<string, bool> { { "error_analyzer", true }, { "performance_profiler", true }, { "memory_analyzer", true }, { "physics_debugger", true }, { "ai_debugger", true }, { "network_debugger", true }, { "save_system_tester", true }, { "event_logger", true }, { "scene_profiler", true }, { "frame_debugger", true } } },
            }
        };
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

        // Preset Panel
        GUILayout.Space(10);
        DrawPresetPanel();

        // Mode Selection
        GUILayout.Space(10);
        DrawModeSelection();

        // Mode Description
        GUILayout.Space(8);
        DrawModeDescription();

        // Tools Panel with Search
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
        GUILayout.Label("Mode Switching, Tool Management & Presets", new GUIStyle(GUI.skin.label) 
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

    private void DrawPresetPanel()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = new Color(0.25f, 0.2f, 0.25f);
        showPresetPanel = EditorGUILayout.Foldout(showPresetPanel, "⚡ PRESET CONFIGURATIONS", true, new GUIStyle(EditorStyles.foldout) { fontSize = 11, fontStyle = FontStyle.Bold });
        GUI.backgroundColor = Color.white;

        if (showPresetPanel)
        {
            GUILayout.Space(8);
            GUILayout.Label("Quick-load pre-configured workflows:", new GUIStyle(GUI.skin.label) { fontSize = 9, fontStyle = FontStyle.Italic, padding = new RectOffset(8, 8, 2, 2) });
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = selectedPreset == "beginner" ? new Color(0.4f, 0.8f, 0.4f) : Color.gray;
            if (GUILayout.Button("👶 Beginner", GUILayout.Height(32)))
            {
                ApplyPreset("beginner");
                selectedPreset = "beginner";
            }
            GUI.backgroundColor = Color.white;

            GUI.backgroundColor = selectedPreset == "artist" ? new Color(1.0f, 0.7f, 0.2f) : Color.gray;
            if (GUILayout.Button("🎨 Artist", GUILayout.Height(32)))
            {
                ApplyPreset("artist");
                selectedPreset = "artist";
            }
            GUI.backgroundColor = Color.white;

            GUI.backgroundColor = selectedPreset == "programmer" ? new Color(0.4f, 0.6f, 1.0f) : Color.gray;
            if (GUILayout.Button("💻 Programmer", GUILayout.Height(32)))
            {
                ApplyPreset("programmer");
                selectedPreset = "programmer";
            }
            GUI.backgroundColor = Color.white;

            GUI.backgroundColor = selectedPreset == "full" ? new Color(1.0f, 0.4f, 0.4f) : Color.gray;
            if (GUILayout.Button("🚀 Full Toolkit", GUILayout.Height(32)))
            {
                ApplyPreset("full");
                selectedPreset = "full";
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            // Show selected preset info
            if (presets.ContainsKey(selectedPreset))
            {
                GUILayout.Space(8);
                var preset = presets[selectedPreset];
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label($"<b>{preset.name}</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 10, fontStyle = FontStyle.Bold, padding = new RectOffset(8, 8, 4, 2) });
                GUILayout.Label(preset.description, new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 9, padding = new RectOffset(8, 8, 2, 4) });
                GUILayout.EndVertical();
            }

            GUILayout.Space(5);
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
                toolSearchFilter = "";
                selectedPreset = "custom";
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
                    toolSearchFilter = "";
                    selectedPreset = "custom";
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

    private void DrawModeDescription()
    {
        int modeIndex = System.Array.IndexOf(MODES, selectedMode);
        if (modeIndex >= 0 && modeIndex < MODE_DESCRIPTIONS.Length)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUI.backgroundColor = new Color(0.2f, 0.25f, 0.3f);
            GUILayout.Label("MODE DESCRIPTION", panelHeaderStyle);
            GUI.backgroundColor = Color.white;
            GUILayout.Label(MODE_DESCRIPTIONS[modeIndex], new GUIStyle(GUI.skin.label) 
            { 
                wordWrap = true,
                padding = new RectOffset(8, 8, 6, 6),
                fontSize = 10,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
            });
            GUILayout.EndVertical();
        }
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

            // Search bar
            GUILayout.BeginHorizontal();
            GUILayout.Label("🔍 Search:", GUILayout.Width(60));
            toolSearchFilter = GUILayout.TextField(toolSearchFilter, GUI.skin.textField);
            if (GUILayout.Button("✕", GUILayout.Width(25)))
            {
                toolSearchFilter = "";
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Quick action buttons
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.4f, 0.7f, 0.4f);
            if (GUILayout.Button("Enable All", GUILayout.Height(25)))
            {
                EnableAllTools();
            }
            GUI.backgroundColor = new Color(0.9f, 0.4f, 0.4f);
            if (GUILayout.Button("Disable All", GUILayout.Height(25)))
            {
                DisableAllTools();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            var tools = modeTools[selectedMode];
            var filteredTools = tools.Where(t => string.IsNullOrEmpty(toolSearchFilter) || 
                                                  t.label.ToLower().Contains(toolSearchFilter.ToLower()) ||
                                                  t.description.ToLower().Contains(toolSearchFilter.ToLower())).ToList();

            GUILayout.Label($"Showing {filteredTools.Count} of {tools.Count} tools", new GUIStyle(GUI.skin.label) { fontSize = 9, fontStyle = FontStyle.Italic, padding = new RectOffset(8, 8, 2, 2) });
            GUILayout.Space(5);

            if (filteredTools.Count == 0)
            {
                GUILayout.Label("No tools match your search.", new GUIStyle(GUI.skin.label) { padding = new RectOffset(8, 8, 4, 4) });
            }
            else
            {
                toolScrollPosition = GUILayout.BeginScrollView(toolScrollPosition, GUILayout.Height(300));
                foreach (var tool in filteredTools)
                {
                    DrawToolToggle(tool);
                }
                GUILayout.EndScrollView();
            }
        }

        GUILayout.EndVertical();
    }

    private void DrawToolToggle(ToolToggle tool)
    {
        string key = $"{selectedMode}_{tool.name}";
        bool currentState = toolStates.ContainsKey(key) ? toolStates[key] : tool.enabled;

        GUILayout.BeginVertical(toolItemStyle);
        
        GUILayout.BeginHorizontal();
        bool newState = EditorGUILayout.Toggle(currentState, GUILayout.Width(20));
        if (newState != currentState)
        {
            toolStates[key] = newState;
            selectedPreset = "custom";
            if (mcpServer != null)
            {
                mcpServer.ToggleTool(tool.name, newState);
            }
        }

        GUILayout.BeginVertical();
        GUILayout.Label(tool.label, new GUIStyle(GUI.skin.label) { padding = new RectOffset(4, 4, 2, 0), fontStyle = FontStyle.Bold });
        GUILayout.Label(tool.description, new GUIStyle(GUI.skin.label) { padding = new RectOffset(4, 4, 0, 2), fontSize = 8, fontStyle = FontStyle.Italic, normal = { textColor = new Color(0.8f, 0.8f, 0.8f) }, wordWrap = true });
        GUILayout.EndVertical();
        
        GUI.backgroundColor = currentState ? new Color(0.4f, 0.7f, 0.4f) : new Color(0.5f, 0.5f, 0.5f);
        GUILayout.Label(currentState ? "ON" : "OFF", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight, fontStyle = FontStyle.Bold }, GUILayout.Width(35));
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
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

    private void ApplyPreset(string presetKey)
    {
        if (!presets.ContainsKey(presetKey))
            return;

        var preset = presets[presetKey];
        selectedMode = preset.defaultMode;

        // Apply tool configurations from preset
        foreach (var modeConfig in preset.toolConfigs)
        {
            foreach (var toolConfig in modeConfig.Value)
            {
                string key = $"{modeConfig.Key}_{toolConfig.Key}";
                toolStates[key] = toolConfig.Value;

                if (mcpServer != null)
                {
                    mcpServer.ToggleTool(toolConfig.Key, toolConfig.Value);
                }
            }
        }

        if (mcpServer != null)
        {
            mcpServer.SetMode(selectedMode);
        }

        Debug.Log($"[MCP Config] Applied preset: {preset.name}");
    }

    private void EnableAllTools()
    {
        if (modeTools.ContainsKey(selectedMode))
        {
            foreach (var tool in modeTools[selectedMode])
            {
                string key = $"{selectedMode}_{tool.name}";
                toolStates[key] = true;
                if (mcpServer != null)
                {
                    mcpServer.ToggleTool(tool.name, true);
                }
            }
            selectedPreset = "custom";
            Debug.Log($"[MCP Config] Enabled all tools in {selectedMode} mode.");
        }
    }

    private void DisableAllTools()
    {
        if (modeTools.ContainsKey(selectedMode))
        {
            foreach (var tool in modeTools[selectedMode])
            {
                string key = $"{selectedMode}_{tool.name}";
                toolStates[key] = false;
                if (mcpServer != null)
                {
                    mcpServer.ToggleTool(tool.name, false);
                }
            }
            selectedPreset = "custom";
            Debug.Log($"[MCP Config] Disabled all tools in {selectedMode} mode.");
        }
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
            toolSearchFilter = "";
            selectedPreset = "custom";
            
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
    public string description;
}

public class ToolPreset
{
    public string name;
    public string description;
    public string defaultMode;
    public Dictionary<string, Dictionary<string, bool>> toolConfigs;
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