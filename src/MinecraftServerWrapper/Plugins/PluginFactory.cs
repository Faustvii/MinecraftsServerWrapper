using McMaster.NETCore.Plugins;
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
    public class PluginFactory
    {
        public Dictionary<IPlugin, bool> Plugins = new Dictionary<IPlugin, bool>();
        public IEnumerable<IPlugin> Enabled => Plugins.Where(x => x.Value).Select(x => x.Key);
        public IEnumerable<IPlugin> Disabled => Plugins.Where(x => !x.Value).Select(x => x.Key);

        private readonly List<PluginLoader> _pluginLoaders = new List<PluginLoader>();

        private readonly string _pluginsDir;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _mainProvider;

        public PluginFactory(IOptions<Settings> options, IMediator mediator, IServiceProvider serviceProvider)
        {
            _pluginsDir = options.Value.PluginsFolder;
            _mediator = mediator;
            _mainProvider = serviceProvider;
        }

        public void Load()
        {
            if (!Directory.Exists(_pluginsDir))
                return;

            if (_pluginLoaders.Any())
            {
                return;
            }

            // create plugin loaders
            var pluginsDir = Path.GetFullPath(_pluginsDir);
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir);
                var pluginDll = Path.Combine(dir, dirName + ".dll");
                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        isUnloadable: true,
                        sharedTypes: new Type[0],
                        configure: config => config.EnableHotReload = true);

                    loader.Reloaded += PluginReloaded;
                    _pluginLoaders.Add(loader);
                }
            }

            var serviceCollection = new ServiceCollection();
            var pluginAssemblies = _pluginLoaders.Select(x => x.LoadDefaultAssembly()).ToArray();

            ConfigurePlugins(serviceCollection, pluginAssemblies);
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
                Message = $"Successfully loaded {plugins.Count()} plugins from {_pluginLoaders.Count} DLLs",
                LogLevel = LogLevel.Information
            });
        }

        private void ConfigurePlugins(IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton(_mainProvider.GetService<IMediator>());
            services.AddSingleton<IMinecraftServer>(_mainProvider.GetService<MinecraftServer>());
            services.AddSingleton<IMinecraftDiscordClient>(_mainProvider.GetService<WonderlandClient>());
            services.AddSingleton<IPluginContext>(x => new PluginContext
            {
                MinecraftProperties = _mainProvider.GetService<MinecraftProperties>(),
                MinecraftSettings = _mainProvider.GetService<IOptions<MinecraftSettings>>().Value,
                MinecraftServer = _mainProvider.GetService<MinecraftServer>(),
                Logger = _mainProvider.GetService<PluginLogger>()
            });

            services.Scan(x =>
            {
                x.FromAssemblies(assemblies).AddClasses(plug => plug.AssignableTo<IPlugin>()).AsImplementedInterfaces().WithSingletonLifetime();
            });
        }

        private void PluginReloaded(object sender, PluginReloadedEventArgs eventArgs)
        {
            var pluginAssembly = eventArgs.Loader.LoadDefaultAssembly();
            var pluginTypes = pluginAssembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
            var pluginsToRemove = Plugins.Where(p => pluginTypes.Any(x => x.FullName == p.Key.GetType().FullName));

            foreach (var plugin in pluginsToRemove)
            {
                Plugins.Remove(plugin.Key);
            }

            var serviceCollection = new ServiceCollection();
            ConfigurePlugins(serviceCollection, pluginAssembly);
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
                Message = $"Successfully reloaded {plugins.Count()} plugins",
                LogLevel = LogLevel.Information
            });

        }

        public void Unload()
        {
            var pluginCount = Plugins.Count;
            Plugins.Clear();

            if (_pluginLoaders.Any())
            {
                foreach (var loader in _pluginLoaders)
                {
                    loader.Dispose();
                }
            }
            _pluginLoaders.Clear();
            _mediator.Publish(new PluginLogNotification
            {
                Message = $"Unloaded {pluginCount} plugins",
                LogLevel = LogLevel.Information
            });
        }
    }
}
