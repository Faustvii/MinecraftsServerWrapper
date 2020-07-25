using System;
using System.Threading.Tasks;

namespace MinecraftServerWrapper.Models.Plugins
{
    public interface IPluginLogger
    {
        Task WarningAsync(string Message);
        Task InformationAsync(string Message);
        Task ErrorAsync(Exception exception, string Message);
    }
}
