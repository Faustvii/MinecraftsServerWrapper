using Discord;
using MediatR;

namespace MinecraftServerWrapper.Models.Events.Discord
{
    public class DiscordLogNotification : INotification
    {
        public LogMessage Log { get; set; }
        public string Message => Log.ToString();
    }
}
