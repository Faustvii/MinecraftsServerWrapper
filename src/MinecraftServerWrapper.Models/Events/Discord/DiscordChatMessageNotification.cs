using MediatR;

namespace MinecraftServerWrapper.Models.Events.Discord
{
    public class DiscordChatMessageNotification : INotification
    {
        public string Username { get; set; }
        public string Message { get; set; }
    }
}
