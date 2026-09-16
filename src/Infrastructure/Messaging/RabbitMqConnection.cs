using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Infrastructure.Options;

namespace Infrastructure.Messaging;

public sealed class RabbitMqConnection(IOptions<RabbitMq> options) : IRabbitMqConnection, IAsyncDisposable
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is not null)
        {
            return _connection;
        }

        await _connectionLock.WaitAsync();

        try
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
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _connectionLock.Dispose();
    }
}
