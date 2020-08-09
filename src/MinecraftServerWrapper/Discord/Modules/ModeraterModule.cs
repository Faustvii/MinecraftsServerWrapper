using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Discord.Modules
{
    [Group("admin")]
    [RequireContext(ContextType.Guild)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Group("clean")]
        public class CleanModule : ModuleBase<SocketCommandContext>
        {
            // ~admin clean
            [Command]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            [RequireBotPermission(GuildPermission.ManageMessages)]
            public async Task DefaultCleanAsync()
            {
                var messages = await Context.Channel.GetMessagesAsync(100).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            }

            // ~admin clean messages 15
            [Command("messages")]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            [RequireBotPermission(GuildPermission.ManageMessages)]
            public async Task CleanAsync(int count)
            {
                var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            }
        }
        // ~admin ban foxbot#0282
        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public Task BanAsync(IGuildUser user) =>
            Context.Guild.AddBanAsync(user);
    }
}
