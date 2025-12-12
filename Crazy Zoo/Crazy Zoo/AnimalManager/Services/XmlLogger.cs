using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AnimalManager.Interfaces;

namespace AnimalManager.Services
{
    /// <summary>
    /// XML implementation of ILogger
    /// Saves logs in XML format
    /// </summary>
    public class XmlLogger : ILogger
    {
        private readonly List<LogEntry> _logs;

        public XmlLogger()
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
                var root = new XElement("Logs");

                foreach (var log in _logs)
                {
                    root.Add(new XElement("LogEntry",
                        new XElement("Timestamp", log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XElement("Message", log.Message)
                    ));
                }

                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save XML log: {ex.Message}", ex);
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                _logs.Clear();

                foreach (var element in doc.Root.Elements("LogEntry"))
                {
                    var timestampStr = element.Element("Timestamp")?.Value;
                    var message = element.Element("Message")?.Value;

                    if (DateTime.TryParse(timestampStr, out DateTime timestamp))
                    {
                        _logs.Add(new LogEntry
                        {
                            Timestamp = timestamp,
                            Message = message
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load XML log: {ex.Message}", ex);
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
    }
}
