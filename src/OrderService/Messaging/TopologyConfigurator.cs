using Infrastructure.Messaging.Contracts;
using RabbitMQ.Client;

namespace HelloRabbitMq.Messaging;

public class TopologyConfigurator(IChannelPool channelPool) : ITopologyConfigurator
{
    public async Task ConfigureAsync(CancellationToken cancellationToken)
    {
        var channel = await channelPool.GetChannelAsync(cancellationToken);

        await DeclareExchange(channel, "orders.events", ExchangeType.Topic, cancellationToken);
        await DeclareQueue(channel, "payments", cancellationToken);
        await BindQueue(channel, "payments", "orders.events", "order.created", cancellationToken);
    }

    private async Task BindQueue(IChannel channel, string queue, string exchange, string routingKey, CancellationToken cancellationToken)
    {
        await channel.QueueBindAsync(
            queue: queue,
            exchange: exchange,
            routingKey: routingKey,
            cancellationToken: cancellationToken);
    }

    private async Task DeclareExchange(IChannel channel, string exchange, string type, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: type,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }

    private async Task DeclareQueue(IChannel channel, string queue, CancellationToken cancellationToken)
    {
        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }
}
