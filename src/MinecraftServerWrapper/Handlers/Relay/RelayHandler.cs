using MediatR;
using MinecraftServerWrapper.Models.Events.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Events.Plugin;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers.Relay
{
    public class RelayHandler :
        INotificationHandler<ServerOutputNotification>, INotificationHandler<ServerStartingNotification>,
        INotificationHandler<ServerStoppedNotification>, INotificationHandler<MinecraftStateNotification>,
        INotificationHandler<PlayerJoinedNotification>, INotificationHandler<PlayerLeftNotification>,
        INotificationHandler<DiscordLogNotification>, INotificationHandler<PluginLogNotification>
    {
        private readonly RelayEventHandler _relayEventHandler;

        public RelayHandler(RelayEventHandler relayEventHandler)
        {
            _relayEventHandler = relayEventHandler;
        }

        public async Task Handle(ServerOutputNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.ServerOutputNotification(notification);
        }

        public async Task Handle(ServerStartingNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.ServerStartingNotification(notification);
        }

        public async Task Handle(ServerStoppedNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.ServerStoppedNotification(notification);
        }

        public async Task Handle(MinecraftStateNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.MinecraftStateNotification(notification);
        }

        public async Task Handle(PlayerJoinedNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.PlayerJoinedNotification(notification);
        }

        public async Task Handle(PlayerLeftNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.PlayerLeftNotification(notification);
        }

        public async Task Handle(DiscordLogNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.DiscordLogNotification(notification);
        }

        public async Task Handle(PluginLogNotification notification, CancellationToken cancellationToken)
        {
            await _relayEventHandler.PluginLogNotification(notification);
        }
    }
}
