using MinecraftServerWrapper.Models.Events.Minecraft;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Models.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual Task OnChatMessage(PlayerChatNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnLoad(IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerAdvancement(PlayerAdvancementNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerDeath(PlayerDiedNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerJoin(PlayerJoinedNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPlayerLeft(PlayerLeftNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStart(ServerStartedNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStop(ServerStoppedNotification notification, IPluginContext context)
        {
            return Task.CompletedTask;
        }
    }
}
