using Infrastructure.Messaging.Contracts;
using RabbitMQ.Client;
using System.Text.Json;

namespace Infrastructure.Messaging;

public class RabbitMqMessageSender(IChannelPool channelPool, IMessageRouter messageRouter) : IMessageSender
{
    public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
    {
        var channel = await channelPool.GetChannelAsync(cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var route = messageRouter.GetRoute(message);

        await channel.BasicPublishAsync(
            exchange: route.Exchange,
            routingKey: route.RoutingKey,
            body: body,
            cancellationToken);
    }
}
