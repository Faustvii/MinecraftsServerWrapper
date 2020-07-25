using MediatR;
using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class PlayerDiedNotification : INotification
    {
        public ServerMessage RawMessage { get; set; }
        public string PlayerName { get; set; }
        public string DeathMessage { get; set; }
    }
}
