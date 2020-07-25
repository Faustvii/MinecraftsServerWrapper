namespace MinecraftServerWrapper.Models.Minecraft
{
    public interface IMinecraftSettings
    {
        string ServerFolderPath { get; }
        string JavaArguments { get; }
    }
}
