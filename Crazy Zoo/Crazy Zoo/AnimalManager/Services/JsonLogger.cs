using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AnimalManager.Interfaces;

namespace AnimalManager.Services
{
    /// <summary>
    /// JSON implementation of ILogger
    /// Saves logs in JSON format
    /// </summary>
    public class JsonLogger : ILogger
    {
        private readonly List<LogEntry> _logs;

        public JsonLogger()
        {
            _logs = new List<LogEntry>();
        }

        public void Log(string message)
        {
            _logs.Add(new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message
            });
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var json = JsonSerializer.Serialize(new { Logs = _logs }, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save JSON log: {ex.Message}", ex);
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<LogContainer>(json);

                _logs.Clear();
                if (data?.Logs != null)
                {
                    _logs.AddRange(data.Logs);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load JSON log: {ex.Message}", ex);
            }
        }

        public IEnumerable<string> GetLogs()
        {
            return _logs.Select(l => $"[{l.Timestamp:HH:mm:ss}] {l.Message}");
        }

        public void Clear()
        {
            _logs.Clear();
        }

        private class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
        }

        private class LogContainer
        {
            public List<LogEntry> Logs { get; set; }
        }
    }
}
