using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Minecraft;
using MinecraftServerWrapper.Models.Minecraft.ServerCommands;
using MinecraftServerWrapper.Models.Plugins;
using NbtLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SamplePlugins
{
    public class DeathLocation : BasePlugin
    {
        public DeathLocation(IPluginContext pluginContext) : base(pluginContext)
        {
        }

        public override string Name => "Death Location";
        public override string Description => "Whispers the location where the player died to them in chat";

        public override async Task OnPlayerDeath(PlayerDiedNotification notification, IPluginContext context)
        {
            var serverWorkingDirectory = context.MinecraftServer.WorkingDirectory;
            if (!File.Exists(Path.Combine(serverWorkingDirectory, "usercache.json")))
            {
                await context.Logger.WarningAsync($"Could not find usercache.json in {serverWorkingDirectory}");
                return;
            }
            // We save the game to have latest NBT data to find the death location.
            await context.MinecraftServer.SendCommandWaitForEventAsync("save-all", "saved the game");
            var player = notification.PlayerName.Trim();

            var usercacheJson = await File.ReadAllTextAsync(Path.Combine(serverWorkingDirectory, "usercache.json"));

            var items = JsonSerializer.Deserialize<UserCacheItem[]>(usercacheJson);
            var uuid = items.FirstOrDefault(x => x.Name == player)?.UUID;
            if (string.IsNullOrWhiteSpace(uuid))
            {
                await context.Logger.WarningAsync($"Could not find {player} in usercache.json");
                return;
            }

            var levelName = context.MinecraftProperties.LevelName;
            if (string.IsNullOrWhiteSpace(levelName))
                return;

            var playerData = Path.Combine(serverWorkingDirectory, levelName, $"playerdata/{uuid}.dat");
            if (!File.Exists(playerData))
            {
                await context.Logger.WarningAsync($"Could not find playerdata with uuid '{uuid}' in {Path.Combine(serverWorkingDirectory, levelName, "playerdata")}");
                return;
            }

            using (var inputStream = File.OpenRead(playerData))
            {
                var nbtData = NbtConvert.DeserializeObject<MinecraftPlayerData>(inputStream);
                var message = $"[Death Location] You died at approximately ({nbtData.Coords})";
                var minecraftTellRaw = new MinecraftTellRaw(message, color: MinecraftColor.red);
                await context.MinecraftServer.SendCommandAsync(minecraftTellRaw.ToCommand(notification.PlayerName));
                await context.Logger.InformationAsync($"Sent death location to {player} for coords {nbtData.Coords}");
            }
        }

        private class UserCacheItem
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("uuid")]
            public string UUID { get; set; }
        }

        private class MinecraftPlayerData
        {
            [NbtProperty(PropertyName = "Pos", UseArrayType = false, EmptyListAsEnd = true)]
            public List<double> Position { get; set; }

            [NbtIgnore]
            public string Coords => string.Join(", ", Position.Select(x => (int)x));
        }
    }
}
