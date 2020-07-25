using MinecraftServerWrapper.Models.Events.Minecraft;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Models.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Short description of what the plugin does
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        Task OnLoad(IPluginContext context);
        /// <summary>
        /// Called when the server starts
        /// </summary>
        Task OnStart(ServerStartedNotification notification, IPluginContext context);
        /// <summary>
        /// Called when the server stops
        /// </summary>
        Task OnStop(ServerStoppedNotification notification, IPluginContext context);
        /// <summary>
        /// Called when a chat message is sent
        /// </summary>
        Task OnChatMessage(PlayerChatNotification notification, IPluginContext context);
        /// <summary>
        /// Called when a player joins the server.
        /// </summary>
        Task OnPlayerJoin(PlayerJoinedNotification notification, IPluginContext context);
        /// <summary>
        /// Called when a player leaves the server.
        /// </summary>
        Task OnPlayerLeft(PlayerLeftNotification notification, IPluginContext context);
        /// <summary>
        /// Called when a player dies.
        /// </summary>
        Task OnPlayerDeath(PlayerDiedNotification notification, IPluginContext context);
        /// <summary>
        /// Called when a player gets an advancement/challenge.
        /// </summary>
        Task OnPlayerAdvancement(PlayerAdvancementNotification notification, IPluginContext context);
    }
}
