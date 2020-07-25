using MediatR;
using Microsoft.Extensions.Options;
using MinecraftServerWrapper.Models.Events.Minecraft;
using MinecraftServerWrapper.Models.Minecraft;
using MinecraftServerWrapper.Models.Settings;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Minecraft
{
    /// <summary>
    /// A wrapper for the Minecraft server process. Provides events and methods for indirect interface with the server process.
    /// </summary>
    public class MinecraftServer : IMinecraftServer
    {
        /// <summary>
        /// The current working directory, the directory that holds server .jar file
        /// </summary>
        public string WorkingDirectory => ServerProcess.StartInfo.WorkingDirectory;
        /// <summary>
        /// The Minecraft server process
        /// </summary>
        private Process ServerProcess { get; set; }

        /// <summary>
        /// Identifies whether the server process is currently running
        /// </summary>
        public bool Running
        {
            get
            {
                if (ServerProcess == null) return false;
                try { Process.GetProcessById(ServerProcess.Id); }
                catch (InvalidOperationException) { return false; }
                catch (ArgumentException) { return false; }
                return true;
            }
        }

        private readonly MinecraftSettings _settings;
        private readonly IMediator _mediator;

        /// <summary>
        /// Creates a new Minecraft server process wrapper
        /// </summary>
        /// <param name="jarFilePath">Path to the server.jar file</param>
        public MinecraftServer(IOptions<MinecraftSettings> options, IMediator mediator)
        {
            _settings = options.Value;
            _mediator = mediator;
        }

        /// <summary>
        /// Creates (but does not start) a new Minecraft server process and adds event handlers
        /// </summary>
        private void CreateServerProcess(Process existingProcess = null)
        {
            ServerProcess = existingProcess ?? new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _settings.ServerFolderPath,
                    Arguments = _settings.JavaArguments,
                    FileName = _settings.JavaExecutable,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            ServerProcess.Exited += ServerProcess_Exited;
            ServerProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
            ServerProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
        }

        private async void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            await _mediator.Publish(new ServerOutputNotification
            {
                Line = e.Data,
                OutputType = OutputType.Error
            });
        }

        private async void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            await _mediator.Publish(new ServerOutputNotification
            {
                Line = e.Data,
                OutputType = OutputType.Standard
            });
        }

        private async void ServerProcess_Exited(object sender, EventArgs e)
        {
            await _mediator.Publish(new ServerStoppedNotification
            {

            });
        }

        /// <summary>
        /// Starts the server process and starts the standard output/error stream if it has not already been started
        /// </summary>
        private void StartServerProcess()
        {
            if (Running)
                return;

            ServerProcess.Start();

            try
            {
                ServerProcess.BeginOutputReadLine();
                ServerProcess.BeginErrorReadLine();
            }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Starts the Minecraft server process if it is not already running
        /// </summary>
        public Task StartAsync()
        {
            if (Running)
                return Task.CompletedTask; ;

            CreateServerProcess();
            StartServerProcess();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the Minecraft server process if it is currently running
        /// </summary>
        public async Task StopAsync()
        {
            if (!Running)
                return;

            await SendCommandAsync("stop");
        }

        /// <summary>
        /// Stops the Minecraft server process if it is currently running and then starts it again
        /// </summary>
        public async Task RestartAsync()
        {
            if (!Running)
                return;

            ServerProcess.Exited += (s, e) => StartAsync();
            await StopAsync();
        }

        /// <summary>
        /// Sends a command to the Minecraft server
        /// </summary>
        /// <param name="text">The command to send</param>
        public async Task SendCommandAsync(string text)
        {
            if (!Running)
                return;

            ServerProcess.StandardInput.WriteLine(text);
            await ServerProcess.StandardInput.FlushAsync();
        }

        private ConcurrentQueue<string> WaitQueue = new ConcurrentQueue<string>();
        private SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private string CommandFinishedPattern { get; set; }

        /// <summary>
        /// Sends a command to the Minecraft server
        /// </summary>
        /// <param name="text">The command to send</param>
        public async Task SendCommandWaitForEventAsync(string text, string eventPattern)
        {
            if (!Running)
                return;

            await Semaphore.WaitAsync();
            CommandFinishedPattern = eventPattern;
            ServerProcess.OutputDataReceived += WaitQueueProcessor;

            await SendCommandAsync(text);

            var timeout = TimeSpan.FromSeconds(10).TotalMilliseconds;
            var elapsed = 0;
            while (WaitQueue.IsEmpty && elapsed < timeout)
            {
                await Task.Delay(25);
                elapsed += 25;
            }

            ServerProcess.OutputDataReceived -= WaitQueueProcessor;
            WaitQueue.Clear();
            Semaphore.Release();
        }

        private void WaitQueueProcessor(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.ContainsIgnoreCase(CommandFinishedPattern))
            {
                WaitQueue.Enqueue(e.Data);
            }
        }
    }
}
