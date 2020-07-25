using MediatR;
using MinecraftServerWrapper.Models.Events.Plugin;
using MinecraftServerWrapper.Models.Plugins;
using System;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Plugins
{
    public class PluginLogger : IPluginLogger
    {
        private readonly IMediator _mediator;

        public PluginLogger(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ErrorAsync(Exception exception, string Message)
        {
            await _mediator.Publish(new PluginLogNotification
            {
                Exception = exception,
                LogLevel = LogLevel.Error,
                Message = Message
            });
        }

        public async Task InformationAsync(string Message)
        {
            await _mediator.Publish(new PluginLogNotification
            {
                LogLevel = LogLevel.Information,
                Message = Message
            });
        }

        public async Task WarningAsync(string Message)
        {
            await _mediator.Publish(new PluginLogNotification
            {
                LogLevel = LogLevel.Warning,
                Message = Message
            });
        }
    }
}
