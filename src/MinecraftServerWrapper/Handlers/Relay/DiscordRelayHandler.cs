using MediatR;
using MinecraftServerWrapper.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers.Relay
{
    public class DiscordRelayHandler :
        INotificationHandler<ServerStartingNotification>, INotificationHandler<ServerStoppedNotification>,
        INotificationHandler<PlayerDiedNotification>, INotificationHandler<PlayerChatNotification>, INotificationHandler<PlayerAdvancementNotification>,
        INotificationHandler<PlayerNotWhitelistedNotification>, INotificationHandler<PlayerWhitelistedNotification>,
        INotificationHandler<MinecraftStateNotification>
    {
        private readonly WonderlandClient _wonderlandClient;

        public DiscordRelayHandler(WonderlandClient wonderlandClient)
        {
            _wonderlandClient = wonderlandClient;
        }

        public async Task Handle(ServerStoppedNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient.SetServerOfflineAsync();
        }

        public async Task Handle(ServerStartingNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient.SetServerStartingAsync();
        }

        public async Task Handle(PlayerDiedNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient.SendMessageAsync($"**{notification.PlayerName}** {notification.DeathMessage}");
        }

        public async Task Handle(PlayerChatNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient?.SendChatMessageAsync($"**<{notification.PlayerName.Trim()}>** {notification.Message.Trim()}");
        }

        public async Task Handle(PlayerAdvancementNotification notification, CancellationToken cancellationToken)
        {
            if (notification.IsChallenge)
            {
                await _wonderlandClient.SendMessageAsync($"**{notification.PlayerName}** has completed the challenge **{notification.Advancement}**");
            }
            else
            {
                await _wonderlandClient.SendMessageAsync($"**{notification.PlayerName}** has made the advancement **{notification.Advancement}**");
            }
        }

        public async Task Handle(PlayerWhitelistedNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient.Minecraft_PlayerWhitelisted().Invoke(notification);

        }

        public async Task Handle(PlayerNotWhitelistedNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient.SendWhitelistMessageAsync(notification);

        }

        public async Task Handle(MinecraftStateNotification notification, CancellationToken cancellationToken)
        {
            await _wonderlandClient.SetCurrentPlayerCountAsync(notification.CurrentState.CurrentPlayerCount, notification.CurrentState.MaxPlayerCount);
        }
    }
}
