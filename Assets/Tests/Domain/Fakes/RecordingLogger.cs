using Domain.Ports.Driven;
using System.Collections.Generic;

namespace Domain.Tests
{
    // An ILogger that records messages instead of printing them.
    public class RecordingLogger : ILogger
    {
        public readonly List<string> Infos = new List<string>();
        public readonly List<string> Warnings = new List<string>();
        public readonly List<string> Errors = new List<string>();

        public void LogInfo(string message) => Infos.Add(message);
        public void LogWarning(string message) => Warnings.Add(message);
        public void LogError(string message) => Errors.Add(message);
    }
}
