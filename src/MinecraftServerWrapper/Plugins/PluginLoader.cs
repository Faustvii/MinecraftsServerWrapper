using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinecraftServerWrapper.Discord;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Models.Discord;
using MinecraftServerWrapper.Models.Events.Plugin;
using MinecraftServerWrapper.Models.Minecraft;
using MinecraftServerWrapper.Models.Plugins;
using MinecraftServerWrapper.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MinecraftServerWrapper.Plugins
{
    public class PluginLoader
    {
        public Dictionary<IPlugin, bool> Plugins = new Dictionary<IPlugin, bool>();
        public IEnumerable<IPlugin> Enabled => Plugins.Where(x => x.Value).Select(x => x.Key);
        public IEnumerable<IPlugin> Disabled => Plugins.Where(x => !x.Value).Select(x => x.Key);

        private readonly string _pluginPath;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _severWrapperProvider;

        public PluginLoader(IOptions<Settings> options, IMediator mediator, IServiceProvider serviceProvider)
        {
            _pluginPath = options.Value.PluginsFolder;
            _mediator = mediator;
            _severWrapperProvider = serviceProvider;
        }

        public void Load()
        {
            if (!Directory.Exists(_pluginPath))
                return;

            var pluginDlls = Directory.GetFiles(_pluginPath).Where(x => Path.GetExtension(x) == ".dll").ToList();

            if (pluginDlls.Count == 0)
                return;

            var serviceCollection = new ServiceCollection();
            ConfigurePlugins(serviceCollection, pluginDlls.Select(x => Assembly.LoadFile(Path.GetFullPath(x))).ToArray());
            var provider = serviceCollection.BuildServiceProvider();

            var plugins = provider.GetServices<IPlugin>();

            foreach (var plugin in plugins)
            {
                Plugins.Add(plugin, true);
                _mediator.Publish(new PluginLogNotification
                {
                    Message = $"Loaded {plugin.Name} ({plugin.Description})",
                    LogLevel = LogLevel.Information
                });
            }

            _mediator.Publish(new PluginLogNotification
            {
                Message = $"Successfully loaded {plugins.Count()} plugins from {pluginDlls.Count} DLLs",
                LogLevel = LogLevel.Information
            });

            return;
        }

        private void ConfigurePlugins(IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton(_severWrapperProvider.GetService<IMediator>());
            services.AddSingleton<IMinecraftServer>(_severWrapperProvider.GetService<MinecraftServer>());
            services.AddSingleton<IMinecraftDiscordClient>(_severWrapperProvider.GetService<WonderlandClient>());
            services.AddSingleton<IPluginContext>(x => new PluginContext
            {
                MinecraftProperties = _severWrapperProvider.GetService<MinecraftProperties>(),
                MinecraftSettings = _severWrapperProvider.GetService<IOptions<MinecraftSettings>>().Value,
                MinecraftServer = _severWrapperProvider.GetService<MinecraftServer>(),
                Logger = _severWrapperProvider.GetService<PluginLogger>()
            });

            services.Scan(x =>
            {
                x.FromAssemblies(assemblies).AddClasses(plug => plug.AssignableTo<IPlugin>()).AsImplementedInterfaces().WithSingletonLifetime();
            });
        }


        public void Unload()
        {
            var pluginCount = Plugins.Count;
            Plugins.Clear();
            _mediator.Publish(new PluginLogNotification
            {
                Message = $"Unloaded {pluginCount} plugins",
                LogLevel = LogLevel.Information
            });
        }

        public void Reload()
        {
            Unload();
            Load();
        }

        public void EnableAll()
        {
            foreach (var plugin in Plugins.Keys)
            {
                Plugins[plugin] = true;
            }
        }

        public void DisableAll()
        {
            foreach (var plugin in Plugins.Keys)
            {
                Plugins[plugin] = false;
            }
        }

        public void Enable(IPlugin plugin)
        {
            if (Plugins.ContainsKey(plugin))
            {
                Plugins[plugin] = true;
            }
        }

        public void Disable(IPlugin plugin)
        {
            if (Plugins.ContainsKey(plugin))
            {
                Plugins[plugin] = false;
            }
        }
    }
}