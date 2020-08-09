using MediatR;
using System;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class ServerCrashedEvent : INotification
    {
        public readonly string CrashReportPath;
        public readonly DateTimeOffset Timestamp;

        public ServerCrashedEvent(string crashReportPath)
        {
            CrashReportPath = crashReportPath;
            Timestamp = DateTimeOffset.Now;
        }
    }
}
