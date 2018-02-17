using System;
using Aquarium.Log;

namespace Aquarium
{
    public interface ILogger
    {
        bool LogInfo { get; set; }
        string GetLogPath();
        void Write(string message, LoggerTypes.LogLevel logLevel = LoggerTypes.LogLevel.Error);
        void Write(Exception e);
    }
}