using MediatR;
using System;

namespace MinecraftServerWrapper.Models.Events.Plugin
{
    public class PluginLogNotification : INotification
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }
}
