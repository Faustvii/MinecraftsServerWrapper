using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Settings
{
    public class Settings
    {
        public DiscordSettings Discord { get; set; }
        public MinecraftSettings Minecraft { get; set; }
        public string PluginsFolder { get; set; } = "Plugins";
    }

    public class DiscordSettings
    {
        public string Token { get; set; }
        public ulong GuildId { get; set; }
        public ulong CommandChannelId { get; set; }
        public ulong ChatChannelId { get; set; }
        public string Prefix { get; set; } = "!";
        public bool UpdateActivity { get; set; } = false;
    }

    public class MinecraftSettings : IMinecraftSettings
    {
        public string JavaExecutable { get; set; } = "Javaw";
        public string ServerFolderPath { get; set; }
        public string JavaArguments { get; set; }
    }
}
