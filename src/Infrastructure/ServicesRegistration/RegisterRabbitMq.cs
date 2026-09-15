using Infrastructure.Messaging;
using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Options;

namespace Infrastructure.ServicesRegistration;

public static class RegisterRabbitMq
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration, Action<RouterBuilder> routerBuilderOptions)
    {
        var routerBuilder = ConfigureRouterBuilder(routerBuilderOptions);

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IChannelPool, ChannelPool>();
        services.AddTransient<IMessageSender, RabbitMqMessageSender>();
        services.AddSingleton(typeof(ITopologyConfigurator), routerBuilder.TopologyConfigurator);

        services.AddSingleton<IMessageRouter, MessageRouter>(sp =>
        {
            var messageRouter = new MessageRouter();
            messageRouter.SetMessagesConfig(routerBuilder.MessagesConfig);

            return messageRouter;
        });

        services.AddHostedService<TopologyConfiguratorHostedService>();

        ConfigureSection(services, configuration);

        return services;
    }

    private static RouterBuilder ConfigureRouterBuilder(Action<RouterBuilder> configureRouterBuilder)
    {
        var routerBuilder = new RouterBuilder();
        configureRouterBuilder.Invoke(routerBuilder);

        if (!EnsureTopologyConfiguratorType(routerBuilder))
        {
            throw new ArgumentException($"{nameof(RouterBuilder.TopologyConfigurator)} type must be assignable to {nameof(ITopologyConfigurator)}");
        }

        return routerBuilder;
    }

    private static bool EnsureTopologyConfiguratorType(RouterBuilder routerBuilder)
    {
        return routerBuilder.TopologyConfigurator.IsAssignableTo(typeof(ITopologyConfigurator));
    }

    private static void ConfigureSection(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmq = configuration.GetRequiredSection(nameof(RabbitMq));
        services.Configure<RabbitMq>(rabbitmq);
    }
}
