using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServerWrapper.Discord;
using MinecraftServerWrapper.Handlers.Relay;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Models.Minecraft;
using MinecraftServerWrapper.Models.Settings;
using MinecraftServerWrapper.Plugins;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MinecraftServerWrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
             .AddUserSecrets(Assembly.GetExecutingAssembly())
#endif
             ;

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // configure settings

            services.Configure<Settings>(Configuration);
            services.Configure<MinecraftSettings>(Configuration.GetSection(nameof(Settings.Minecraft)));
            services.Configure<DiscordSettings>(Configuration.GetSection(nameof(Settings.Discord)));

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(MainWindow));

            services.AddSingleton(new DiscordSocketClient())
                    .AddSingleton(new CommandService(new CommandServiceConfig
                    {
                        DefaultRunMode = RunMode.Async
                    }))
                    .AddSingleton<CommandHandler>();

            services.AddSingleton<WonderlandClient>();
            services.AddSingleton<MinecraftServer>();
            services.AddSingleton<MinecraftProperties>();
            services.AddSingleton<MinecraftState>();
            services.AddSingleton<RelayEventHandler>();
            //services.AddSingleton<PluginLoader>();
            services.AddSingleton<PluginFactory>();
            services.AddSingleton<PluginLogger>();
        }
    }
}
