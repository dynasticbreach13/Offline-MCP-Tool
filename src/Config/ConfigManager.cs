using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Unity3DMCP
{
    /// <summary>
    /// Manages JSON configuration loading and parsing
    /// </summary>
    public class ConfigManager
    {
        private JObject configData;
        private string configPath;

        public ConfigManager(string path)
        {
            configPath = path;
            LoadConfiguration();
        }

        /// <summary>
        /// Load configuration from JSON file
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                TextAsset configFile = Resources.Load<TextAsset>(configPath.Replace("Assets/Resources/", "").Replace(".json", ""));
                
                if (configFile == null)
                {
                    Debug.LogError("[ConfigManager] Config file not found at: " + configPath);
                    return;
                }

                configData = JObject.Parse(configFile.text);
                Debug.Log("[ConfigManager] Configuration loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("[ConfigManager] Failed to load configuration: " + ex.Message);
            }
        }

        /// <summary>
        /// Get the default mode from config
        /// </summary>
        public string GetDefaultMode()
        {
            return configData?["defaultMode"]?.Value<string>() ?? "master";
        }

        /// <summary>
        /// Get server name from config
        /// </summary>
        public string GetServerName()
        {
            return configData?["server"]?["name"]?.Value<string>() ?? "Unity3D-MCP-Server";
        }

        /// <summary>
        /// Get server version from config
        /// </summary>
        public string GetServerVersion()
        {
            return configData?["server"]?["version"]?.Value<string>() ?? "1.0.0";
        }

        /// <summary>
        /// Get mode configuration by ID
        /// </summary>
        public ModeConfig GetModeConfig(string modeId)
        {
            var modeData = configData?["modes"]?[modeId];
            if (modeData == null)
            {
                return null;
            }

            var modeConfig = new ModeConfig
            {
                modeId = modeId,
                displayName = modeData["displayName"]?.Value<string>() ?? modeId,
                description = modeData["description"]?.Value<string>() ?? "",
                emoji = modeData["emoji"]?.Value<string>() ?? "",
                enabled = modeData["enabled"]?.Value<bool>() ?? true,
                tools = new Dictionary<string, ToolConfig>()
            };

            // Load tool configurations
            var toolsData = modeData["tools"];
            if (toolsData != null)
            {
                foreach (var toolProp in toolsData)
                {
                    var toolId = toolProp.Key;
                    var toolData = toolProp.Value;

                    modeConfig.tools[toolId] = new ToolConfig
                    {
                        toolId = toolId,
                        enabled = toolData["enabled"]?.Value<bool>() ?? false,
                        description = toolData["description"]?.Value<string>() ?? ""
                    };
                }
            }

            return modeConfig;
        }

        /// <summary>
        /// Get all mode IDs from config
        /// </summary>
        public List<string> GetAllModeIds()
        {
            var modes = new List<string>();
            var modesData = configData?["modes"];
            if (modesData != null)
            {
                foreach (var modeProp in modesData)
                {
                    modes.Add(modeProp.Key);
                }
            }
            return modes;
        }

        /// <summary>
        /// Get server host from config
        /// </summary>
        public string GetServerHost()
        {
            return configData?["server"]?["host"]?.Value<string>() ?? "127.0.0.1";
        }

        /// <summary>
        /// Get server port from config
        /// </summary>
        public int GetServerPort()
        {
            return configData?["server"]?["port"]?.Value<int>() ?? 3000;
        }

        /// <summary>
        /// Check if logging is enabled
        /// </summary>
        public bool IsLoggingEnabled()
        {
            return configData?["logging"]?["enabled"]?.Value<bool>() ?? true;
        }

        /// <summary>
        /// Get logging level
        /// </summary>
        public string GetLoggingLevel()
        {
            return configData?["logging"]?["level"]?.Value<string>() ?? "info";
        }

        /// <summary>
        /// Update tool state in configuration
        /// </summary>
        public bool UpdateToolState(string modeId, string toolId, bool enabled)
        {
            try
            {
                configData["modes"][modeId]["tools"][toolId]["enabled"] = enabled;
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("[ConfigManager] Failed to update tool state: " + ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// Configuration for a game development mode
    /// </summary>
    public class ModeConfig
    {
        public string modeId;
        public string displayName;
        public string description;
        public string emoji;
        public bool enabled;
        public Dictionary<string, ToolConfig> tools;
    }

    /// <summary>
    /// Configuration for a tool
    /// </summary>
    public class ToolConfig
    {
        public string toolId;
        public bool enabled;
        public string description;
    }
}
