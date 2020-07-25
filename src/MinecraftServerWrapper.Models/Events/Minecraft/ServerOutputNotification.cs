using MediatR;

namespace MinecraftServerWrapper.Models.Events.Minecraft
{
    public class ServerOutputNotification : INotification
    {
        public string Line { get; set; }
        public OutputType OutputType { get; set; }
    }

    public enum OutputType
    {
        Standard,
        Error
    }
}
