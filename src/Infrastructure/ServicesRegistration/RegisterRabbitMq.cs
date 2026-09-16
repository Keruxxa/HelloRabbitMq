using Infrastructure.Messaging;
using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Options;

namespace Infrastructure.ServicesRegistration;

public static class RegisterRabbitMq
{
    /// <remarks>
    ///     <c>Note:</c> If you provide <paramref name="configureRouterBuilder"/>, you have to configure both <see cref="RouterBuilder.MessagesConfig"/> and <see cref="RouterBuilder.TopologyConfigurator"/>
    /// </remarks>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<RouterBuilder>? configureRouterBuilder = null)
    {
        var routerBuilder = configureRouterBuilder is null
            ? null
            : ConfigureRouterBuilder(configureRouterBuilder);

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IChannelPool, ChannelPool>();
        services.AddTransient<IMessageSender, RabbitMqMessageSender>();

        services.AddSingleton<IMessageRouter, MessageRouter>(_ =>
        {
            var messageRouter = new MessageRouter();

            if (routerBuilder is not null)
            {
                messageRouter.SetMessagesConfig(routerBuilder.MessagesConfig);
            }

            return messageRouter;
        });

        if (routerBuilder is not null)
        {
            services.AddSingleton(typeof(ITopologyConfigurator), routerBuilder.TopologyConfigurator);
            services.AddHostedService<TopologyConfiguratorHostedService>();
        }

        ConfigureSection(services, configuration);

        return services;
    }

    public static IServiceCollection AddRabbitMqConsumer<TMessage, TConsumer>(
        this IServiceCollection services,
        Action<RabbitMqConsumerOptions<TMessage>>? configureOptions = null)
        where TConsumer : class, IMessageConsumer<TMessage>
    {
        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }

        services.AddScoped<IMessageConsumer<TMessage>, TConsumer>();
        services.AddHostedService<RabbitMqConsumerHostedService<TMessage>>();

        return services;
    }

    private static RouterBuilder ConfigureRouterBuilder(Action<RouterBuilder> configureRouterBuilder)
    {
        var routerBuilder = new RouterBuilder();
        configureRouterBuilder.Invoke(routerBuilder);

        if (!routerBuilder.TopologyConfigurator.IsAssignableTo(typeof(ITopologyConfigurator)))
        {
            throw new ArgumentException($"{nameof(RouterBuilder.TopologyConfigurator)} type must be assignable to {nameof(ITopologyConfigurator)}");
        }

        return routerBuilder;
    }

    private static void ConfigureSection(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmq = configuration.GetRequiredSection(nameof(RabbitMq));
        services.Configure<RabbitMq>(rabbitmq);
    }
}
