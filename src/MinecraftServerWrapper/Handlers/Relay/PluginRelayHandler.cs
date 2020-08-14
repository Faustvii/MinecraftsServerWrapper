using MediatR;
using Microsoft.Extensions.Options;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Events.Plugin;
using MinecraftServerWrapper.Models.Minecraft;
using MinecraftServerWrapper.Models.Plugins;
using MinecraftServerWrapper.Models.Settings;
using MinecraftServerWrapper.Plugins;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers.Relay
{
    internal class PluginRelayHandler :
        INotificationHandler<ServerStoppedNotification>, INotificationHandler<ServerStartedNotification>,
        INotificationHandler<PlayerDiedNotification>, INotificationHandler<PlayerAdvancementNotification>,
        INotificationHandler<PlayerJoinedNotification>, INotificationHandler<PlayerLeftNotification>, INotificationHandler<PlayerChatNotification>, INotificationHandler<ServerCrashedEvent>
    {
        private readonly PluginFactory _pluginLoader;
        private readonly IMediator _mediator;
        private readonly PluginContext _pluginContext;

        public PluginRelayHandler(PluginFactory pluginLoader, MinecraftProperties minecraftProperties, IOptions<MinecraftSettings> minecraftSettings, IMediator mediator, MinecraftServer minecraftServer, PluginLogger pluginLogger)
        {
            _pluginLoader = pluginLoader;
            _mediator = mediator;
            _pluginContext = new PluginContext
            {
                MinecraftProperties = minecraftProperties,
                MinecraftSettings = minecraftSettings.Value,
                MinecraftServer = minecraftServer,
                Logger = pluginLogger
            };
        }

        public async Task Handle(ServerStartedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnStart(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnStart)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(ServerStoppedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnStop(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnStop)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(PlayerJoinedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnPlayerJoin(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnPlayerJoin)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(PlayerLeftNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnPlayerLeft(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnPlayerLeft)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(PlayerChatNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnChatMessage(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnChatMessage)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(PlayerDiedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnPlayerDeath(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnPlayerDeath)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(PlayerAdvancementNotification notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnPlayerAdvancement(notification, _pluginContext);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnPlayerAdvancement)} for plugin {plugin.Name}"
                    });
                }
            }
        }

        public async Task Handle(ServerCrashedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoader.Enabled)
            {
                try
                {
                    await plugin.OnCrash(notification);
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new PluginLogNotification
                    {
                        Exception = ex,
                        Message = $"Error in {nameof(plugin.OnCrash)} for plugin {plugin.Name}"
                    });
                }
            }
        }
    }
}
