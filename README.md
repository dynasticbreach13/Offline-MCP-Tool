# Unity 3D MCP Server - README

A comprehensive **Model Context Protocol (MCP) server** for Unity 3D game development with **7 specialized game development modes** and **50+ intelligent tools**.

![Unity](https://img.shields.io/badge/Unity-6%2B-black?style=flat-square&logo=unity)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
![Status](https://img.shields.io/badge/Status-Production%20Ready-blue?style=flat-square)

## ✨ Key Features

### 7 Specialized Game Development Modes
- 🎨 **Modeling** - Asset creation, mesh optimization, materials
- 🦴 **Rigging** - Skeleton setup, bone hierarchy, weight painting
- 🎬 **Animations** - Animation clips, state machines, blending
- 📝 **Scripting** - C# generation, API references, best practices
- 📦 **Hierarchy** - Project organization, naming conventions
- 🐛 **Debugging** - Error analysis, performance profiling
- 🎮 **Master** - Full access to all 50+ tools

### Easy Setup & Configuration
```json
{
  "defaultMode": "master",
  "server": {"name": "Unity3D-MCP-Server", "version": "1.0.0"}
}
```

### Dynamic Tool Control
Toggle any tool on/off with simple JSON configuration:
```json
"mesh_analyzer": { "enabled": true }
"script_generator": { "enabled": false }
```

### Offline & Lightweight
- No external API calls required
- Runs completely offline
- Minimal dependencies
- Unity 6+ compatible

## 🚀 Quick Start

### 1. Installation
```bash
# Copy src/ folder to your Unity project
cp -r src/ Assets/Scripts/

# Copy configuration
cp config.example.json Assets/Resources/config.json
```

### 2. Setup in Scene
1. Create empty GameObject
2. Add `MCPServer` component
3. Set config path: `Assets/Resources/config.json`
4. Play scene

### 3. Verify
Check Console for: `[MCP Server] Initialization complete`

## 📖 Documentation

- **[Setup Guide](SETUP_GUIDE.md)** - Installation and basic configuration
- **[Tools Reference](TOOLS_REFERENCE.md)** - Complete tool documentation
- **[Configuration](config.example.json)** - Example configuration

## 🎯 Use Cases

### For Game Developers
- **Asset Pipeline**: Model → Rig → Animate workflow support
- **Code Generation**: Generate boilerplate C# scripts
- **Optimization**: Profile and debug performance issues
- **Organization**: Maintain consistent project structure

### For Artists
- **Mesh Optimization**: Reduce poly count while maintaining quality
- **Rigging Guidance**: Best practices for character setup
- **Material Workflows**: Proper texture and material setup
- **LOD Management**: Generate optimized LOD levels

### For Programmers
- **API Reference**: Quick access to Unity API examples
- **Code Snippets**: Common patterns and implementations
- **Debug Tools**: Performance profiling and error analysis
- **Best Practices**: Architecture and optimization guidance

## 📁 Project Structure

```
Offline-MCP-Tool/
├── README.md                      # This file
├── SETUP_GUIDE.md                 # Setup instructions
├── TOOLS_REFERENCE.md             # Tool documentation
├── config.example.json            # Example configuration
├── src/
│   ├── MCPServer.cs              # Main server
│   ├── Modes/
│   │   ├── GameDevMode.cs        # Base mode class
│   │   ├── ModelingMode.cs       # 🎨 Modeling
│   │   ├── RiggingMode.cs        # 🦴 Rigging
│   │   ├── AnimationMode.cs      # 🎬 Animation
│   │   ├── ScriptingMode.cs      # 📝 Scripting
│   │   ├── HierarchyMode.cs      # 📦 Hierarchy
│   │   ├── DebuggingMode.cs      # 🐛 Debugging
│   │   └── MasterMode.cs         # 🎮 Master
│   ├── Tools/
│   │   ├── ToolBase.cs           # Base tool class
│   │   └── ToolRegistry.cs       # Tool management
│   └── Config/
│       └── ConfigManager.cs      # Configuration
└── Documentation/
    ├── SETUP_GUIDE.md
    └── TOOLS_REFERENCE.md
```

## 🔧 Configuration Examples

### Modeling Focus
```json
{
  "defaultMode": "modeling",
  "modes": {
    "modeling": {
      "tools": {
        "mesh_analyzer": { "enabled": true },
        "material_helper": { "enabled": true },
        "script_generator": { "enabled": false }
      }
    }
  }
}
```

### Scripting Focus
```json
{
  "defaultMode": "scripting",
  "modes": {
    "scripting": {
      "tools": {
        "script_generator": { "enabled": true },
        "api_reference": { "enabled": true },
        "mesh_analyzer": { "enabled": false }
      }
    }
  }
}
```

## 💻 Runtime API

### Switch Modes
```csharp
MCPServer server = MCPServer.Instance;
server.SetMode("rigging");
```

### Toggle Tools
```csharp
server.ToggleTool("mesh_analyzer", false);
```

### Get Available Tools
```csharp
List<ToolInfo> tools = server.GetAvailableTools();
foreach (var tool in tools)
{
    Debug.Log($"{tool.name}: {tool.description}");
}
```

### Execute Tools
```csharp
var result = await server.ExecuteTool("mesh_analyzer", 
    new Dictionary<string, object> { { "mesh_name", "MyMesh" } });
```

### Get Server Status
```csharp
var status = server.GetStatus();
Debug.Log($"Current Mode: {status.currentMode}");
Debug.Log($"Enabled Tools: {status.enabledToolCount}");
```

## 📊 Statistics

| Metric | Count |
|--------|-------|
| **Modes** | 7 |
| **Tools** | 50+ |
| **Modeling Tools** | 9 |
| **Rigging Tools** | 7 |
| **Animation Tools** | 8 |
| **Scripting Tools** | 9 |
| **Hierarchy Tools** | 8 |
| **Debugging Tools** | 10 |

## 🛠️ Requirements

- **Unity Version**: 6.0 or higher
- **Scripting Backend**: Any (.NET Framework or IL2CPP)
- **Dependencies**: Newtonsoft.Json (for JSON parsing)

## 📝 License

MIT License - Feel free to use in commercial projects

## 🤝 Contributing

Contributions welcome! Areas for expansion:
- Additional tool implementations
- New game development modes
- Performance optimizations
- Documentation improvements

## 📞 Support

For issues or questions:
1. Check [Setup Guide](SETUP_GUIDE.md)
2. Review [Tools Reference](TOOLS_REFERENCE.md)
3. Verify `config.json` syntax
4. Check Console output for error messages

## 🎯 Roadmap

- [ ] Visual UI for mode selection
- [ ] Tool dependency management
- [ ] Custom tool creation framework
- [ ] Export/import configurations
- [ ] Analytics and usage tracking
- [ ] Community tool marketplace

---

**Built for Unity game developers, by game developers.** 🎮

Transform your Unity 3D workflow with intelligent, specialized tools designed for every aspect of game development.
