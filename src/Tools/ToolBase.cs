using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity3DMCP
{
    /// <summary>
    /// Base class for all MCP tools
    /// </summary>
    public abstract class ToolBase
    {
        /// <summary>
        /// Get unique tool identifier
        /// </summary>
        public abstract string GetToolId();

        /// <summary>
        /// Get human-readable tool name
        /// </summary>
        public abstract string GetToolName();

        /// <summary>
        /// Get tool description
        /// </summary>
        public virtual string GetToolDescription()
        {
            return $"Tool for {GetToolName()}";
        }

        /// <summary>
        /// Get input schema (JSON Schema format)
        /// </summary>
        protected abstract Dictionary<string, object> GetInputSchema();

        /// <summary>
        /// Execute the tool with given parameters
        /// </summary>
        public async Task<ToolResult> Execute(Dictionary<string, object> parameters)
        {
            try
            {
                ValidateParameters(parameters);
                return await OnExecute(parameters);
            }
            catch (ArgumentException ex)
            {
                return new ToolResult
                {
                    success = false,
                    error = $"Invalid parameters: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ToolResult
                {
                    success = false,
                    error = $"Tool execution error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Override to implement tool logic
        /// </summary>
        protected abstract Task<ToolResult> OnExecute(Dictionary<string, object> parameters);

        /// <summary>
        /// Validate parameters against schema
        /// </summary>
        protected virtual void ValidateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Get a typed parameter with default fallback
        /// </summary>
        protected T GetParameter<T>(Dictionary<string, object> parameters, string key, T defaultValue)
        {
            if (parameters != null && parameters.ContainsKey(key))
            {
                try
                {
                    return (T)Convert.ChangeType(parameters[key], typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Get tool information
        /// </summary>
        public ToolInfo GetToolInfo()
        {
            return new ToolInfo
            {
                id = GetToolId(),
                name = GetToolName(),
                description = GetToolDescription(),
                inputSchema = GetInputSchema(),
                enabled = true
            };
        }
    }
}
