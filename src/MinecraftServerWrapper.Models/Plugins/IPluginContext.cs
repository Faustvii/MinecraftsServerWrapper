using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Plugins
{
    public interface IPluginContext
    {
        IMinecraftServer MinecraftServer { get; }
        IMinecraftProperties MinecraftProperties { get; }
        IMinecraftSettings MinecraftSettings { get; }
        IPluginLogger Logger { get; }
    }
}
