using Microsoft.Extensions.DependencyInjection;

namespace MinecraftServerWrapper.Models.Plugins
{
    public interface IPluginDependencyInjection
    {
        /// <summary>
        /// You can register services that your plugins need, these will be registered in DI before your plugins are loaded.
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);
    }
}
