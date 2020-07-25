using System.Threading.Tasks;

namespace MinecraftServerWrapper.Models.Minecraft
{
    public interface IMinecraftServer
    {
        string WorkingDirectory { get; }
        bool Running { get; }
        Task StartAsync();
        Task StopAsync();
        Task RestartAsync();
        Task SendCommandAsync(string text);
        Task SendCommandWaitForEventAsync(string text, string eventPattern);
    }
}
