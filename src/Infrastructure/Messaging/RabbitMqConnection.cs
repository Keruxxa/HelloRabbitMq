using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Infrastructure.Options;

namespace Infrastructure.Messaging;

public class RabbitMqConnection(IOptions<RabbitMq> options) : IRabbitMqConnection, IAsyncDisposable
{
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is not null)
        {
            return _connection;
        }

        var rabbitMq = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = rabbitMq.HostName,
            UserName = rabbitMq.UserName,
            Password = rabbitMq.Password,
        };

        _connection = await factory.CreateConnectionAsync();

        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
