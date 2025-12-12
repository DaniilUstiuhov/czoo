using System.Collections.Generic;

namespace AnimalManager.Interfaces
{
    /// <summary>
    /// Interface for logging services
    /// Supports saving logs to different formats (XML, JSON, etc.)
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Add a log entry
        /// </summary>
        void Log(string message);

        /// <summary>
        /// Save all logs to file
        /// </summary>
        void SaveToFile(string filePath);

        /// <summary>
        /// Load logs from file
        /// </summary>
        void LoadFromFile(string filePath);

        /// <summary>
        /// Get all log entries
        /// </summary>
        IEnumerable<string> GetLogs();

        /// <summary>
        /// Clear all logs
        /// </summary>
        void Clear();
    }
}
