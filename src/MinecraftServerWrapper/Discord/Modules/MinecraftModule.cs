using Discord;
using Discord.Commands;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Models.Minecraft;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Discord.Modules
{
    [Name("Minecraft")]
    public class MinecraftModule : ModuleBase<SocketCommandContext>
    {
        private readonly MinecraftState _minecraftState;
        private readonly MinecraftServer _minecraftServer;

        public MinecraftModule(MinecraftState minecraftState, MinecraftServer minecraftServer)
        {
            _minecraftState = minecraftState;
            _minecraftServer = minecraftServer;
        }

        [Command("whitelist")]
        [Summary("Whitelists a user on the minecraft server")]
        [RequireRole("Admin")]
        public async Task SayAsync([Remainder][Summary("The minecraft username to whitelist")] string minecraftName)
        {

            if (!_minecraftState.Running)
            {
                await ReplyAsync("The server is currently not running, please try once it's running");
            }
            else
            {
                await ReplyAsync($"Whitelisting {minecraftName}");
                await _minecraftServer.SendCommandAsync($"whitelist add {minecraftName}");
                await ReplyAsync($"{minecraftName} was whitelisted");
            }
        }

        [Command("list")]
        [Summary("Shows a list of players online on the server")]
        public async Task PlayersAsync()
        {
            var players = _minecraftState.CurrentPlayers.Select(x => $"**{x}**").ToList();

            var builder = new EmbedBuilder();

            builder.WithTitle($"There's currently {players.Count}/{_minecraftState.MaxPlayerCount} players online");
            builder.WithDescription(string.Join(Environment.NewLine, players));
            builder.WithColor(Color.DarkGreen);

            await ReplyAsync("", false, builder.Build());
        }
    }
}
