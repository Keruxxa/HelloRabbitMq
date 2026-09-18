using Infrastructure.Messaging.Contracts;
using RabbitMQ.Client;

namespace PaymentConsumer.Messaging;

public class PaymentTopologyConfigurator(IChannelPool channelPool) : ITopologyConfigurator
{
    public async Task ConfigureAsync(CancellationToken cancellationToken)
    {
        var channel = await channelPool.GetChannelAsync(cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: "orders.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: "notifications",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: "notifications",
            exchange: "orders.events",
            routingKey: string.Empty,
            cancellationToken: cancellationToken);
    }
}
