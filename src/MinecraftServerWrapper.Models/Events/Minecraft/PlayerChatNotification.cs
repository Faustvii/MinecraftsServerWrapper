using MediatR;
using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class PlayerChatNotification : INotification
    {
        public ServerMessage RawMessage { get; set; }
        public string PlayerName { get; set; }
        public string Message { get; set; }
    }
}
