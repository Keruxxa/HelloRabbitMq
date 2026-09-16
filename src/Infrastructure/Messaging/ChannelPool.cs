using Infrastructure.Messaging.Contracts;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Infrastructure.Messaging;

public sealed class ChannelPool(IRabbitMqConnection rabbitMqConnection) : IChannelPool, IAsyncDisposable
{
    private readonly ConcurrentBag<IChannel> _channels = new();

    public async ValueTask<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        while (_channels.TryTake(out var channel))
        {
            if (channel.IsOpen)
            {
                return channel;
            }

            await channel.DisposeAsync();
        }

        var connection = await rabbitMqConnection.GetConnectionAsync();

        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask Return(IChannel channel)
    {
        if (channel.IsOpen)
        {
            _channels.Add(channel);
        }
        else
        {
            await channel.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        while (_channels.TryTake(out var channel))
        {
            await channel.DisposeAsync();
        }
    }
}
