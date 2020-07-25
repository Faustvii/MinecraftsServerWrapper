using MediatR;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Minecraft;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers
{
    public class MinecraftStateHandler : INotificationHandler<PlayerJoinedNotification>, INotificationHandler<PlayerLeftNotification>, INotificationHandler<ServerStoppedNotification>, INotificationHandler<ServerStartedNotification>
    {
        private readonly IMediator _mediator;
        private readonly MinecraftState _state;

        public MinecraftStateHandler(IMediator mediator, MinecraftState state)
        {
            _mediator = mediator;
            _state = state;
        }

        public async Task Handle(PlayerJoinedNotification notification, CancellationToken cancellationToken)
        {
            _state.CurrentPlayers.Add(notification.PlayerName);
            await _mediator.Publish(new MinecraftStateNotification
            {
                CurrentState = _state
            });
        }

        public async Task Handle(PlayerLeftNotification notification, CancellationToken cancellationToken)
        {
            _state.CurrentPlayers.Remove(notification.PlayerName);
            await _mediator.Publish(new MinecraftStateNotification
            {
                CurrentState = _state
            });
        }

        public async Task Handle(ServerStartedNotification notification, CancellationToken cancellationToken)
        {
            _state.Running = true;
            _state.CurrentPlayers.Clear();
            _state.StartTime = DateTimeOffset.Now;
            await _mediator.Publish(new MinecraftStateNotification
            {
                CurrentState = _state
            });
        }

        public async Task Handle(ServerStoppedNotification notification, CancellationToken cancellationToken)
        {
            _state.Running = false;
            _state.CurrentPlayers.Clear();
            _state.StartTime = null;
            await _mediator.Publish(new MinecraftStateNotification
            {
                CurrentState = _state
            });
        }
    }
}
