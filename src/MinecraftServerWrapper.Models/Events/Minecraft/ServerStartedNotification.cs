using MediatR;
using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class ServerStartedNotification : INotification
    {
        public ServerMessage RawMessage { get; set; }
    }
}
