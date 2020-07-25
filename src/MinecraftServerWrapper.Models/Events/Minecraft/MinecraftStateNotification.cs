using MediatR;
using MinecraftServerWrapper.Models.Minecraft;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class MinecraftStateNotification : INotification
    {
        public MinecraftState CurrentState { get; set; }
    }
}
