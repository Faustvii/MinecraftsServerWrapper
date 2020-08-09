using MinecraftServerWrapper.Models.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamplePlugins
{
    public class CrashRecovery : BasePlugin
    {
        private readonly IMinecraftDiscordClient _minecraftDiscordClient;

        public override string Name => "Crash Recovery";
        public override string Description => "This will try to restart the server if it crashes";
        public Queue<DateTimeOffset> CrashRecoveryQueue = new Queue<DateTimeOffset>();

        public CrashRecovery(IMinecraftDiscordClient minecraftDiscordClient, IPluginContext context) : base(context)
        {
            _minecraftDiscordClient = minecraftDiscordClient;
        }

        public override async Task OnCrash(ServerCrashedEvent crashedEvent)
        {
            if (CrashRecoveryQueue.Count != 0)
            {
                await _minecraftDiscordClient.SendMessageAsync("Was unable to successfully start the minecraft server");
                CrashRecoveryQueue.Clear();
                return;
            }
            await _minecraftDiscordClient.SendMessageAsync("The minecraft server crashed with an unhandled exception - attempting to restart the server");
            CrashRecoveryQueue.Enqueue(crashedEvent.Timestamp);
            var timeout = TimeSpan.FromSeconds(10).TotalMilliseconds;
            var elapsed = 0;
            while (PluginContext.MinecraftServer.Running || elapsed > timeout)
            {
                await Task.Delay(250);
                elapsed += 250;
            }

            await PluginContext.MinecraftServer.StartAsync();
        }

        public override async Task OnStart(ServerStartedNotification notification, IPluginContext context)
        {
            if (CrashRecoveryQueue.TryDequeue(out var crashTimestamp))
            {
                await _minecraftDiscordClient.SendMessageAsync($"The minecraft server started successfully after the crash on {crashTimestamp}");
            }
        }
    }
}
