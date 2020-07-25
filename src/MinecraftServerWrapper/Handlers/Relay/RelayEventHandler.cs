using MinecraftServerWrapper.Models.Events.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Events.Plugin;
using System;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers.Relay
{
    public class RelayEventHandler
    {
        public event Func<DiscordLogNotification, Task> DiscordLogEvent;
        public event Func<ServerOutputNotification, Task> ConsoleOutputReceived;
        public event Func<PlayerJoinedNotification, Task> PlayerJoined;
        public event Func<PlayerLeftNotification, Task> PlayerLeft;
        public event Func<MinecraftStateNotification, Task> StateChanged;
        public event Func<ServerStartingNotification, Task> ServerStarting;
        public event Func<ServerStoppedNotification, Task> ServerStopped;
        public event Func<PluginLogNotification, Task> PluginLogEvent;


        internal async Task ServerOutputNotification(ServerOutputNotification notification)
        {
            if (ConsoleOutputReceived != null)
                await ConsoleOutputReceived?.Invoke(notification);
        }

        internal async Task PlayerJoinedNotification(PlayerJoinedNotification notification)
        {
            if (PlayerJoined != null)
                await PlayerJoined?.Invoke(notification);
        }

        internal async Task PlayerLeftNotification(PlayerLeftNotification notification)
        {
            if (PlayerLeft != null)
                await PlayerLeft?.Invoke(notification);
        }

        internal async Task MinecraftStateNotification(MinecraftStateNotification notification)
        {
            if (StateChanged != null)
                await StateChanged?.Invoke(notification);
        }

        internal async Task ServerStartingNotification(ServerStartingNotification notification)
        {
            if (ServerStarting != null)
                await ServerStarting?.Invoke(notification);
        }

        internal async Task ServerStoppedNotification(ServerStoppedNotification notification)
        {
            if (ServerStopped != null)
                await ServerStopped?.Invoke(notification);
        }

        internal async Task DiscordLogNotification(DiscordLogNotification notification)
        {
            if (DiscordLogEvent != null)
                await DiscordLogEvent?.Invoke(notification);
        }

        internal async Task PluginLogNotification(PluginLogNotification notification)
        {
            if (PluginLogEvent != null)
                await PluginLogEvent?.Invoke(notification);
        }
    }
}
