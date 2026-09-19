using Infrastructure.Messaging;
using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Options;

namespace Infrastructure.ServicesRegistration;

public static class RegisterRabbitMq
{
    /// <summary>
    ///     Registers RabbitMQ
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <paramref name="configureRouterBuilder"/> must be provided to define routes in order to use <see cref="IMessageSender"/>
    ///     </para>
    ///     <para>
    ///         <c>Note:</c> If you provide <paramref name="configureRouterBuilder"/>, you have to call both <see cref="RouterBuilder.SetMessagesConfig"/> and <see cref="RouterBuilder.SetTopologyConfigurator"/>
    ///     </para>
    /// </remarks>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRouterBuilder>? configureRouterBuilder = null)
    {
        services.AddSingleton<IMessagesConfig, DefaultMessageConfig>();
        services.AddSingleton<IMessageRouter, MessageRouter>();

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IChannelPool, ChannelPool>();
        services.AddTransient<IMessageSender, RabbitMqMessageSender>();

        if (configureRouterBuilder is not null)
        {
            var routerBuilder = new RouterBuilder(services);
            configureRouterBuilder(routerBuilder);
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

    private static void ConfigureSection(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmq = configuration.GetRequiredSection(nameof(RabbitMq));
        services.Configure<RabbitMq>(rabbitmq);
    }
}
