using MediatR;
using MinecraftServerWrapper.Models.Events.Plugin;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Handlers
{
    public class PluginLogHandler : INotificationHandler<PluginLogNotification>
    {
        public Task Handle(PluginLogNotification notification, CancellationToken cancellationToken)
        {
            if (notification.Exception != null)
            {
                Debug.WriteLine($"{notification.Message} - Exception: {notification.Exception}");
            }
            else
            {
                Debug.WriteLine(notification.Message);
            }
            return Task.CompletedTask;
        }
    }
}
