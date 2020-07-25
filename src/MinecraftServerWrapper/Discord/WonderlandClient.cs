using Discord;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Options;
using MinecraftServerWrapper.Models.Discord;
using MinecraftServerWrapper.Models.Events.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Settings;
using MinecraftServerWrapper.Requests.Minecraft;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Discord
{
    public class WonderlandClient : IMinecraftDiscordClient
    {
        public DiscordSocketClient Client { get; private set; }
        private readonly DiscordSettings _settings;
        private readonly CommandHandler _commandHandler;
        private readonly IMediator _mediator;

        private readonly ConcurrentDictionary<string, IUserMessage> _whitelistQueue = new ConcurrentDictionary<string, IUserMessage>();

        public WonderlandClient(DiscordSocketClient client, CommandHandler commandHandler, IOptions<DiscordSettings> settings, IMediator mediator)
        {
            Client = client;
            _settings = settings.Value;
            _commandHandler = commandHandler;
            _mediator = mediator;

            Client.Log += Log;
            Client.MessageReceived += Client_ReceivedMessage;
            Client.ReactionAdded += Client_ReactionAdded;
        }

        public Func<PlayerWhitelistedNotification, Task> Minecraft_PlayerWhitelisted()
        {
            return async (playerWhitelisted) =>
            {
                if (_whitelistQueue.TryRemove(playerWhitelisted.PlayerName, out var msg))
                {
                    await msg.DeleteAsync();
                }
            };
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == "\u2705")
            {
                if (reaction.User.GetValueOrDefault() is SocketGuildUser user)
                {
                    var msg = await message.GetOrDownloadAsync();
                    if (msg.Author.Id == Client.CurrentUser.Id && msg.Content.Contains("whitelisted") && reaction.UserId != Client.CurrentUser.Id)
                    {
                        if (user.Roles.Any(x => x.Name == "Admin"))
                        {
                            var playerName = Regex.Match(msg.Content, @"^<.+> '(.+?)' is not whitelisted, want to whitelist\?$").Groups[1].Value;
                            _whitelistQueue.TryAdd(playerName, msg);

                            await _mediator.Send(new SendCommandRequest
                            {
                                Command = $"whitelist add {playerName}"
                            });
                        }
                    }
                }
            }
            else if (reaction.Emote.Name == "\u274C")
            {
                if (reaction.User.GetValueOrDefault() is SocketGuildUser user)
                {
                    if (user.Roles.Any(x => x.Name == "Admin"))
                    {
                        var msg = await message.GetOrDownloadAsync();
                        if (msg.Author.Id == Client.CurrentUser.Id && msg.Content.Contains("whitelisted") && reaction.UserId != Client.CurrentUser.Id)
                        {
                            await msg.DeleteAsync();
                        }
                    }
                }
            };
        }

        private async Task Client_ReceivedMessage(SocketMessage arg)
        {
            if (arg.Source == MessageSource.Bot) // ignore all bot messages
                return;

            if (arg.Channel.Id == _settings.ChatChannelId)
            {
                if (string.IsNullOrWhiteSpace(arg.Content))
                    return;

                await _mediator.Publish(new DiscordChatMessageNotification
                {
                    Username = arg.Author.Username,
                    Message = arg.Content
                });
            }
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrWhiteSpace(_settings.Token))
            {
                await _mediator.Publish(new DiscordLogNotification
                {
                    Log = new LogMessage(LogSeverity.Critical, "Start", "Invalid bot token, could not start discord")
                });
                return;
            }
            await Client.LoginAsync(TokenType.Bot, _settings.Token);
            await Client.StartAsync();
            await SetServerOfflineAsync();
            await _commandHandler.InstallCommandsAsync();
        }

        public async Task StopAsync()
        {
            await Client.StopAsync();
        }

        public async Task SendMessageAsync(string msg)
        {
            if (Client.ConnectionState != ConnectionState.Connected)
            {
                return;
            }
            var channel = Client.GetGuild(_settings.GuildId).GetTextChannel(_settings.ChatChannelId);
            await channel?.SendMessageAsync(msg);
        }

        public async Task SendWhitelistMessageAsync(PlayerNotWhitelistedNotification whitelistEvent)
        {
            var guild = Client.GetGuild(_settings.GuildId);
            var role = guild.Roles.FirstOrDefault(x => x.Name == "Admin");
            if (role == null)
                return;


            var channel = guild.GetTextChannel(_settings.CommandChannelId);
            var msg = await channel.SendMessageAsync($"{role.Mention} '{whitelistEvent.PlayerName}' is not whitelisted, want to whitelist?");
            var checkMark = new Emoji("\u2705");
            var cross = new Emoji("\u274C");
            await msg.AddReactionsAsync(new[] { checkMark, cross });
        }

        public async Task SetServerOfflineAsync()
        {
            await SetActivityAsync("is offline");
        }

        public async Task SetServerStartingAsync()
        {
            await SetActivityAsync("is starting");
        }

        public async Task SetCurrentPlayerCountAsync(int currentPlayerCount, int maxPlayerCount)
        {
            await SetActivityAsync($"{currentPlayerCount}/{maxPlayerCount}");
        }

        public async Task SetActivityAsync(string message)
        {
            if (_settings.UpdateActivity)
            {
                await Client.SetActivityAsync(new Game(message));
            }
        }

        private async Task Log(LogMessage msg)
        {
            await _mediator.Publish(new DiscordLogNotification
            {
                Log = msg
            });
        }
    }
}