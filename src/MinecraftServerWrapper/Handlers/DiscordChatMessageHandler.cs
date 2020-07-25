using MediatR;
using MinecraftServerWrapper.Models.Events.Discord;
using MinecraftServerWrapper.Requests.Minecraft;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers
{
    public class DiscordChatMessageHandler : INotificationHandler<DiscordChatMessageNotification>
    {
        private readonly IMediator _mediator;

        public DiscordChatMessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DiscordChatMessageNotification notification, CancellationToken cancellationToken)
        {
            var asciiMessage = Encoding.ASCII.GetString(Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty), new DecoderExceptionFallback()).GetBytes(notification.Message));
            await _mediator.Send(new SendCommandRequest
            {
                Command = $"tellraw @a {{\"text\":\"[{notification.Username}] {asciiMessage}\"}}"
            });
        }
    }
}
