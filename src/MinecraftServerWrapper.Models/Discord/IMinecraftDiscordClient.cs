using Discord.WebSocket;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Models.Discord
{
    public interface IMinecraftDiscordClient
    {
        DiscordSocketClient Client { get; }
        Task SendMessageAsync(string msg);
        Task SetActivityAsync(string message);
        Task StartAsync();
        Task StopAsync();
        Task SetServerOfflineAsync();
        Task SetServerStartingAsync();
        Task SetCurrentPlayerCountAsync(int currentPlayers, int maxPlayers);
    }
}
