using MediatR;
using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class ServerStoppedNotification : INotification
    {
        public ServerMessage RawMessage { get; set; }
    }
}
