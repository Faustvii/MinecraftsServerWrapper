using Microsoft.Extensions.Options;
using MinecraftServerWrapper.Discord;
using MinecraftServerWrapper.Handlers.Relay;
using MinecraftServerWrapper.Minecraft;
using MinecraftServerWrapper.Models.Events.Discord;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Events.Plugin;
using MinecraftServerWrapper.Models.Minecraft;
using MinecraftServerWrapper.Models.Settings;
using MinecraftServerWrapper.Plugins;
using MinecraftServerWrapper.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MinecraftServerWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ConsoleContent _minecraftConsole = new ConsoleContent();
        readonly ConsoleContent _discordConsole = new ConsoleContent();
        readonly ConsoleContent _pluginConsole = new ConsoleContent();
        readonly SettingsManager<Settings> _settingsManager = new SettingsManager<Settings>("appsettings.json");
        readonly Settings _settings;
        readonly MinecraftServer _minecraft;
        readonly WonderlandClient _wonderlandClient;
        private readonly RelayEventHandler _relayEventHandler;
        private readonly PluginFactory _pluginLoader;
        private readonly MinecraftState _minecraftState;

        public MainWindow(MinecraftServer minecraft, WonderlandClient wonderlandClient, IOptions<Settings> settings, RelayEventHandler relayEventHandler, PluginFactory pluginLoader, MinecraftState minecraftState)
        {
            InitializeComponent();
            DataContext = new
            {
                Minecraft = _minecraftConsole,
                Discord = _discordConsole,
                Plugin = _pluginConsole,
                Settings = _settings
            };

            _settings = settings.Value;

            Loaded += MainWindow_Loaded;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _minecraft = minecraft;
            _wonderlandClient = wonderlandClient;
            _relayEventHandler = relayEventHandler;
            _pluginLoader = pluginLoader;
            _minecraftState = minecraftState;
        }

        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_minecraft.Running)
            {
                await _minecraft?.StopAsync();
            }
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var directory = new DirectoryInfo(_settings.Minecraft.ServerFolderPath).Name;
            Title = $"Minecraft Server | {directory} | {_minecraftState.LevelName}";
            PlayerListLabel.Content = $"Players online {_minecraftState.CurrentPlayerCount}/{_minecraftState.MaxPlayerCount}";

            InputBlock.KeyDown += InputBlock_KeyDown;
            SetupEventRelays();

            await LoadSettingsToUI();

            if (_wonderlandClient != null)
            {
                await _wonderlandClient.StartAsync();
            }

            _pluginLoader.Load();
        }

        private void SetupEventRelays()
        {
            _relayEventHandler.ConsoleOutputReceived += Minecraft_ConsoleOutputDataReceived;
            _relayEventHandler.PlayerJoined += Minecraft_PlayerJoined;
            _relayEventHandler.PlayerLeft += Minecraft_PlayerLeft;
            _relayEventHandler.StateChanged += Minecraft_StateChanged;
            _relayEventHandler.ServerStarting += Minecraft_ServerStarting;
            _relayEventHandler.ServerStopped += Minecraft_ServerStopped;
            _relayEventHandler.PluginLogEvent += Plugin_LogEvent;
            _relayEventHandler.DiscordLogEvent += WonderlandClient_LogEvent;
        }

        private async Task Plugin_LogEvent(PluginLogNotification log)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                _pluginConsole.AddOutput(log.Message);
                PluginConsole.ScrollToBottom();
            });
        }

        public async Task Minecraft_ServerStopped(ServerStoppedNotification notification)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                PlayerList.Items?.Clear();
            });
        }

        private async Task LoadSettingsToUI()
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                txtMinecraftServerFolderPath.Text = _settings.Minecraft?.ServerFolderPath;
                txtMinecraftJavaArguments.Text = _settings.Minecraft?.JavaArguments;
                txtJavaExecutable.Text = _settings.Minecraft.JavaExecutable;

                txtDiscordToken.Text = _settings.Discord?.Token;
                txtDiscordGuildId.Text = _settings.Discord?.GuildId.ToString();
                txtDiscordCommandChannelId.Text = _settings.Discord?.CommandChannelId.ToString();
                txtDiscordChatChannelId.Text = _settings.Discord?.ChatChannelId.ToString();
                txtDiscordPrefix.Text = _settings.Discord?.Prefix.ToString();
                checkDiscordActivity.IsChecked = _settings.Discord?.UpdateActivity;

                txtPluginFolder.Text = _settings.PluginsFolder;
            });
        }

        public async Task Minecraft_ServerStarting(ServerStartingNotification notification)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                StartButton.Content = "Stop";
            });
        }

        public async Task Minecraft_StateChanged(MinecraftStateNotification notification)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                PlayerListLabel.Content = $"Players online {notification.CurrentState.CurrentPlayerCount}/{notification.CurrentState.MaxPlayerCount}";
                if (notification.CurrentState.Running)
                {
                    StartButton.Content = "Stop";
                }
                else
                {
                    StartButton.Content = "Start";
                }
            });
        }

        private async Task WonderlandClient_LogEvent(DiscordLogNotification notification)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                _discordConsole.AddOutput(notification.Message);
                DiscordConsole.ScrollToBottom();
            });
        }

        public async Task Minecraft_PlayerLeft(PlayerLeftNotification playerLeftEvent)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
             {
                 PlayerList.Items.Remove(playerLeftEvent.PlayerName);
                 PlayerListLabel.Content = $"Players online {PlayerList.Items.Count}/20";
             });
        }

        public async Task Minecraft_PlayerJoined(PlayerJoinedNotification playerJoinEvent)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                PlayerList.Items.Add(playerJoinEvent.PlayerName);
                PlayerListLabel.Content = $"Players online {PlayerList.Items.Count}/20";
            });
        }

        public async Task Minecraft_ConsoleOutputDataReceived(ServerOutputNotification data)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                _minecraftConsole.AddOutput(data.Line);
                MinecraftConsole.ScrollToBottom();
            });
        }

        async void InputBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _minecraftConsole.ConsoleInput = InputBlock.Text;
                await _minecraft.SendCommandAsync(_minecraftConsole.ConsoleInput);
                _minecraftConsole.RunCommand();
                InputBlock.Focus();
                MinecraftConsole.ScrollToBottom();
            }
        }

        private async void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_minecraft != null)
            {
                if (_minecraft.Running)
                {
                    await _minecraft.StopAsync();
                }
                else
                {
                    await _minecraft.StartAsync();
                }
            }
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("backup click");
            foreach (var plugin in _pluginLoader.Enabled)
            {
                var stackPanel = new StackPanel();
                var grid = new Grid
                {
                    Background = Brushes.Black,
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                grid.Children.Add(new Label { Foreground = Brushes.White, FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Content = plugin.Description });
                var button = new Button { Content = "Click me" };
                button.Click += Button_Click;
                grid.Children.Add(button);

                var tab = new TabItem()
                {
                    Header = plugin.Name,
                    Background = Brushes.Gray,
                    Content = grid
                };


                Tabs.Items.Add(tab);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DAMN!");
        }

        private void UpdateMod_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Update mod clicked");
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_minecraft != null && _minecraft.Running)
            {
                await _minecraft.StopAsync();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_settings.Minecraft == null)
                _settings.Minecraft = new MinecraftSettings();

            if (_settings.Discord == null)
                _settings.Discord = new DiscordSettings();

            _settings.Minecraft.ServerFolderPath = txtMinecraftServerFolderPath.Text;
            _settings.Minecraft.JavaArguments = txtMinecraftJavaArguments.Text;
            _settings.Minecraft.JavaExecutable = txtJavaExecutable.Text;

            _settings.Discord.Token = txtDiscordToken.Text;
            _settings.Discord.Prefix = txtDiscordPrefix.Text;
            _settings.Discord.UpdateActivity = checkDiscordActivity.IsChecked.GetValueOrDefault();

            _settings.PluginsFolder = txtPluginFolder.Text;

            if (ulong.TryParse(txtDiscordGuildId.Text, out var guildId))
                _settings.Discord.GuildId = guildId;

            if (ulong.TryParse(txtDiscordCommandChannelId.Text, out var commandChannelId))
                _settings.Discord.CommandChannelId = commandChannelId;

            if (ulong.TryParse(txtDiscordChatChannelId.Text, out var chatId))
                _settings.Discord.ChatChannelId = chatId;

            _settingsManager.SaveSettings(_settings);
        }

        private void ReloadPlugin_Click(object sender, RoutedEventArgs e)
        {
            _pluginLoader.Load();
        }

        private void OpenPluginFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(_settings.PluginsFolder))
            {
                Directory.CreateDirectory(_settings.PluginsFolder);
            }

            var pluginFolderPath = Path.GetFullPath(_settings.PluginsFolder);
            Process.Start("explorer.exe", pluginFolderPath);
        }

        private void UnloadPlugin_Click(object sender, RoutedEventArgs e)
        {
            _pluginLoader.Unload();
        }
    }
}