using RabbitMQ.Client;

namespace Infrastructure.Messaging.Contracts;

public interface IRabbitMqConnection
{
    public Task<IConnection> GetConnectionAsync();
}
