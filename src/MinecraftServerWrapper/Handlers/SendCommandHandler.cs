using MediatR;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Requests.Minecraft;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers
{
    public class SendCommandHandler : AsyncRequestHandler<SendCommandRequest>
    {
        private readonly MinecraftServer _minecraftServer;

        public SendCommandHandler(MinecraftServer minecraftServer)
        {
            _minecraftServer = minecraftServer;
        }

        protected override async Task Handle(SendCommandRequest request, CancellationToken cancellationToken)
        {
            await _minecraftServer.SendCommandAsync(request.Command);
        }
    }
}
