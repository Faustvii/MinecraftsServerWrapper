using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Plugins
{
    public class PluginContext : IPluginContext
    {
        public IMinecraftProperties MinecraftProperties { get; set; }
        public IMinecraftSettings MinecraftSettings { get; set; }
        public IMinecraftServer MinecraftServer { get; set; }
        public IPluginLogger Logger { get; set; }
    }
}
