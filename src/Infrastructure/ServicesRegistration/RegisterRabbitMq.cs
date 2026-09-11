using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.Infrastructure.Options;

namespace Infrastructure.ServicesRegistration;

public static class RegisterRabbitMq
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmq = configuration.GetRequiredSection(nameof(RabbitMq));

        services.Configure<RabbitMq>(rabbitmq);

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitmq["HostName"]!,
                UserName = rabbitmq["UserName"]!,
                Password = rabbitmq["Password"]!,
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        return services;
    }
}
