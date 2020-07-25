using MediatR;
using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class PlayerAdvancementNotification : INotification
    {
        public ServerMessage RawMessage { get; set; }
        public string PlayerName { get; set; }
        public string Advancement { get; set; }
        public bool IsChallenge { get; set; }
    }
}
