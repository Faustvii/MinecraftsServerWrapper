using MediatR;

namespace MinecraftServerWrapper.Requests.Minecraft
{
    public class SendCommandRequest : IRequest
    {
        public string Command { get; set; }
    }
}
