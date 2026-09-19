using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging;

public class RouterBuilder(IServiceCollection services) : IRouterBuilder
{
    public void SetMessagesConfig<TConfig>() where TConfig : class, IMessagesConfig
    {
        services.AddSingleton<IMessagesConfig, TConfig>();
    }

    public void SetTopologyConfigurator<TConfigurator>() where TConfigurator : class, ITopologyConfigurator
    {
        services.AddSingleton<ITopologyConfigurator, TConfigurator>();
    }
}
