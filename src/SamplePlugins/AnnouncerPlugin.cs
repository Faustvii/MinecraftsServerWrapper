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
        public override string Description => "This will announce Server start/stop on Discord";

        public AnnouncerPlugin(IMinecraftDiscordClient minecraftDiscordClient)
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
    }
}
