using RabbitMQ.Client;

namespace Infrastructure.Messaging.Contracts;

public interface IChannelPool
{
    ValueTask<IChannel> GetChannelAsync(CancellationToken cancellationToken);
    ValueTask Return(IChannel channel);
}
