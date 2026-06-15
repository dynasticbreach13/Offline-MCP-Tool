using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3DMCP
{
    /// <summary>
    /// Registry for managing all available tools
    /// </summary>
    public class ToolRegistry
    {
        private Dictionary<string, ToolBase> registeredTools = new Dictionary<string, ToolBase>();

        /// <summary>
        /// Register a tool
        /// </summary>
        public void RegisterTool(string toolId, ToolBase tool)
        {
            if (registeredTools.ContainsKey(toolId))
            {
                Debug.LogWarning($"[ToolRegistry] Tool '{toolId}' already registered, overwriting");
            }

            registeredTools[toolId] = tool;
            Debug.Log($"[ToolRegistry] Tool registered: {toolId}");
        }

        /// <summary>
        /// Get a registered tool
        /// </summary>
        public ToolBase GetTool(string toolId)
        {
            if (registeredTools.ContainsKey(toolId))
            {
                return registeredTools[toolId];
            }
            return null;
        }

        /// <summary>
        /// Check if a tool is registered
        /// </summary>
        public bool HasTool(string toolId)
        {
            return registeredTools.ContainsKey(toolId);
        }

        /// <summary>
        /// Get all registered tool IDs
        /// </summary>
        public List<string> GetAllToolIds()
        {
            return new List<string>(registeredTools.Keys);
        }

        /// <summary>
        /// Get total number of registered tools
        /// </summary>
        public int GetToolCount()
        {
            return registeredTools.Count;
        }
    }

    /// <summary>
    /// Registry for managing all game development modes
    /// </summary>
    public class ModeRegistry
    {
        private Dictionary<string, GameDevMode> registeredModes = new Dictionary<string, GameDevMode>();

        /// <summary>
        /// Register a mode
        /// </summary>
        public void RegisterMode(GameDevMode mode)
        {
            string modeId = mode.GetModeId();
            if (registeredModes.ContainsKey(modeId))
            {
                Debug.LogWarning($"[ModeRegistry] Mode '{modeId}' already registered, overwriting");
            }

            registeredModes[modeId] = mode;
            Debug.Log($"[ModeRegistry] Mode registered: {modeId} - {mode.GetDisplayName()}");
        }

        /// <summary>
        /// Get a registered mode
        /// </summary>
        public GameDevMode GetMode(string modeId)
        {
            if (registeredModes.ContainsKey(modeId))
            {
                return registeredModes[modeId];
            }
            return null;
        }

        /// <summary>
        /// Check if a mode is registered
        /// </summary>
        public bool HasMode(string modeId)
        {
            return registeredModes.ContainsKey(modeId);
        }

        /// <summary>
        /// Get all registered mode IDs
        /// </summary>
        public List<string> GetAllModeIds()
        {
            return new List<string>(registeredModes.Keys);
        }

        /// <summary>
        /// Get total number of registered modes
        /// </summary>
        public int GetModeCount()
        {
            return registeredModes.Count;
        }

        /// <summary>
        /// Get all registered modes
        /// </summary>
        public Dictionary<string, GameDevMode> GetAllModes()
        {
            return new Dictionary<string, GameDevMode>(registeredModes);
        }
    }
}
