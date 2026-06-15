# SETUP_GUIDE.md - Unity 3D MCP Server

## Quick Start (5 Minutes)

### 1. **Download & Setup**
```
1. Copy the entire `src/` folder to your Unity project's Assets folder
2. Copy `config.example.json` to `Assets/Resources/config.json`
3. Create an empty GameObject in your scene
4. Add the `MCPServer` script to it
5. Set the config path in the inspector
```

### 2. **Basic Configuration**
Copy this to your `config.json`:
```json
{
  "defaultMode": "master",
  "server": {
    "name": "Unity3D-MCP-Server",
    "version": "1.0.0"
  }
}
```

### 3. **Run & Test**
- Play your scene
- Check the Console for "[MCP Server] Initialization complete"
- You're ready to go!

## The 7 Modes

### 🎨 **Modeling** 
Focus on 3D assets, meshes, and materials.
```json
"defaultMode": "modeling"
```
**Key Tools**: mesh_analyzer, material_helper, texture_guide

### 🦴 **Rigging**
Focus on skeleton setup and character rigging.
```json
"defaultMode": "rigging"
```
**Key Tools**: skeleton_builder, weight_painter, constraint_helper

### 🎬 **Animations**
Focus on animation clips and state machines.
```json
"defaultMode": "animations"
```
**Key Tools**: animation_clipper, state_machine, blend_tree

### 📝 **Scripting**
Focus on C# code generation and best practices.
```json
"defaultMode": "scripting"
```
**Key Tools**: script_generator, api_reference, code_snippets

### 📦 **Hierarchy**
Focus on project organization and naming.
```json
"defaultMode": "hierarchy"
```
**Key Tools**: naming_convention, hierarchy_organizer, batch_renamer

### 🐛 **Debugging**
Focus on error analysis and performance.
```json
"defaultMode": "debugging"
```
**Key Tools**: error_analyzer, performance_profiler, memory_analyzer

### 🎮 **Master** (Default)
Full access to all tools. Best for learning.
```json
"defaultMode": "master"
```
**Key Tools**: All 50+ tools available

## Toggling Tools

Simply change `enabled` to control which tools are available:

```json
"modeling": {
  "tools": {
    "mesh_analyzer": { "enabled": true },
    "script_generator": { "enabled": false }
  }
}
```

## Runtime Control

Switch modes in code:
```csharp
MCPServer server = MCPServer.Instance;
server.SetMode("rigging");

// Toggle specific tool
server.ToggleTool("mesh_analyzer", false);

// Get available tools
var tools = server.GetAvailableTools();
```

## File Structure

```
Assets/
├── Resources/
│   └── config.json
├── Scripts/
│   └── src/
│       ├── MCPServer.cs
│       ├── Modes/
│       │   ├── GameDevMode.cs
│       │   ├── ModelingMode.cs
│       │   ├── RiggingMode.cs
│       │   ├── AnimationMode.cs
│       │   ├── ScriptingMode.cs
│       │   ├── HierarchyMode.cs
│       │   ├── DebuggingMode.cs
│       │   └── MasterMode.cs
│       ├── Tools/
│       │   ├── ToolBase.cs
│       │   └── ToolRegistry.cs
│       └── Config/
│           └── ConfigManager.cs
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Config not loading | Place `config.json` in `Assets/Resources/` |
| Tools not showing | Verify tools are enabled in JSON |
| Mode won't switch | Check mode ID matches exactly (case-sensitive) |
| Errors in console | Review JSON syntax, enable Debug logging |

## Features

✅ **7 Specialized Modes** for different workflows  
✅ **50+ Tools** across all modes  
✅ **JSON Configuration** - Copy & paste setup  
✅ **Tool Toggles** - Enable/disable any tool  
✅ **Runtime Switching** - Change modes on the fly  
✅ **Offline** - No external dependencies  
✅ **Unity 6+** - Built for latest versions  

## Next Steps

1. **Explore Master Mode** - Test all tools and capabilities
2. **Customize Config** - Disable tools you don't need
3. **Create Focused Modes** - Setup different configs per task
4. **Integrate Tools** - Use returned data in your workflow

---

**Ready to accelerate your Unity 3D development!** 🚀
