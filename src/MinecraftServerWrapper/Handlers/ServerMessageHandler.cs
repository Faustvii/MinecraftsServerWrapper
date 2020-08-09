using MediatR;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Minecraft;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers
{
    public class ServerMessageHandler : INotificationHandler<ServerOutputNotification>
    {
        private readonly IMediator _mediator;

        public ServerMessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ServerOutputNotification notification, CancellationToken cancellationToken)
        {
            if (notification == null || notification.Line == null || string.IsNullOrWhiteSpace(notification.Line))
                return;

            var rawText = notification.Line;

            if (Regex.IsMatch(rawText, @"^\[.+\].+\[.+\]:(.+)$"))
            {
                var match = Regex.Match(rawText, @"^\[.+\].+\[.+\]:(.+)$");
                var message = match.Groups[1].Value.Trim();
                if (Regex.IsMatch(message, @"^(.+?) left the game$"))
                {
                    var playerName = Regex.Match(message, @"^(.+?) left the game$").Groups[1].Value;

                    await _mediator.Publish(new PlayerLeftNotification
                    {
                        PlayerName = playerName,
                        RawMessage = ServerMessage.Create(rawText)
                    });
                }
                else if (Regex.IsMatch(message, @"^(.+?) joined the game$"))
                {
                    var playerName = Regex.Match(message, @"^(.+?) joined the game$").Groups[1].Value;

                    await _mediator.Publish(new PlayerJoinedNotification
                    {
                        PlayerName = playerName,
                        RawMessage = ServerMessage.Create(rawText)
                    });
                }
                else if (Regex.IsMatch(message, @"^<(.+)>(.+)$"))
                {
                    var groups = Regex.Match(message, @"^<(.+)>(.+)$").Groups;
                    var playerName = groups[1].Value;
                    var playerMessage = groups[2].Value;

                    await _mediator.Publish(new PlayerChatNotification
                    {
                        PlayerName = playerName,
                        Message = playerMessage,
                        RawMessage = ServerMessage.Create(rawText)
                    });
                }
                else if (Regex.IsMatch(message, @"^Done.+!.+For.+$"))
                {
                    await _mediator.Publish(new ServerStartedNotification
                    {
                        RawMessage = ServerMessage.Create(rawText)
                    });
                }
                else if (Regex.IsMatch(message, @"^Stopping the server$") || Regex.IsMatch(message, @"^Failed to start.+$"))
                {
                    await _mediator.Publish(new ServerStoppedNotification
                    {
                        RawMessage = ServerMessage.Create(rawText)
                    });
                }
                else if (Regex.IsMatch(message, @"^Loading for game.+$"))
                {
                    await _mediator.Publish(new ServerStartingNotification
                    {
                        RawMessage = ServerMessage.Create(rawText)
                    });
                }
                else if (Regex.IsMatch(message, @"^(.+?) has made the advancement.+(\[.+\])$") || Regex.IsMatch(message, @"^(.+?) has completed the challenge.+(\[.+\])$"))
                {
                    var groups = Regex.Match(message, @"^(.+?) .+(\[.+\])$").Groups;
                    var playerName = groups[1].Value;
                    var advacementName = groups[2].Value;
                    var isChallenge = false;

                    if (message.Contains("challenge", StringComparison.OrdinalIgnoreCase))
                        isChallenge = true;

                    await _mediator.Publish(new PlayerAdvancementNotification
                    {
                        RawMessage = ServerMessage.Create(rawText),
                        Advancement = advacementName,
                        PlayerName = playerName,
                        IsChallenge = isChallenge
                    });
                }
                else if (Regex.IsMatch(message, @"^Disconnecting.+name=(.+?),.+You are not white-listed on this server!$"))
                {
                    var playerName = Regex.Match(message, @"^Disconnecting.+name=(.+?),.+You are not white-listed on this server!$").Groups[1].Value;

                    await _mediator.Publish(new PlayerNotWhitelistedNotification
                    {
                        RawMessage = ServerMessage.Create(rawText),
                        PlayerName = playerName
                    });

                }
                else if (Regex.IsMatch(message, @"^Added (.+?) to the whitelist$"))
                {
                    var playerName = Regex.Match(message, @"^Added (.+?) to the whitelist$").Groups[1].Value;

                    await _mediator.Publish(new PlayerWhitelistedNotification
                    {
                        RawMessage = ServerMessage.Create(rawText),
                        PlayerName = playerName
                    });
                }
                else if (message.IsPlayerDeathMessage())
                {
                    var playerName = message.Split(' ').First().Trim();
                    var deathMessage = string.Join(' ', message.Split(' ').Skip(1));

                    await _mediator.Publish(new PlayerDiedNotification
                    {
                        RawMessage = ServerMessage.Create(rawText),
                        PlayerName = playerName,
                        DeathMessage = deathMessage
                    });
                }
                else if (Regex.IsMatch(message, @"^This crash report .+: (.+)$"))
                {
                    var crashReport = Regex.Match(message, @"^This crash report .+: (.+)$").Groups[1].Value;
                    await _mediator.Publish(new ServerCrashedEvent(crashReport));
                }
            }
        }
    }
}
