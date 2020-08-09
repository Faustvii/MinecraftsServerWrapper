using MinecraftServerWrapper.Models.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Plugins;
using System.Threading.Tasks;

namespace SamplePlugins
{
    public class AnnouncerPlugin : BasePlugin
    {
        private readonly IMinecraftDiscordClient _minecraftDiscordClient;

        public override string Name => "Announcer";
        public override string Description => "This will announce Server start/stop and player join/left on Discord";

        public AnnouncerPlugin(IMinecraftDiscordClient minecraftDiscordClient, IPluginContext context) : base(context)
        {
            _minecraftDiscordClient = minecraftDiscordClient;
        }

        public override async Task OnStart(ServerStartedNotification notification, IPluginContext context)
        {
            await _minecraftDiscordClient.SendMessageAsync("Server started");
        }

        public override async Task OnStop(ServerStoppedNotification notification, IPluginContext context)
        {
            await _minecraftDiscordClient.SendMessageAsync("Server stopped");
        }

        public override async Task OnPlayerJoin(PlayerJoinedNotification notification, IPluginContext context)
        {
            await _minecraftDiscordClient.SendMessageAsync($"{notification.PlayerName} has joined the game.");
        }

        public override async Task OnPlayerLeft(PlayerLeftNotification notification, IPluginContext context)
        {
            await _minecraftDiscordClient.SendMessageAsync($"{notification.PlayerName} has left the game.");
        }
    }
}
