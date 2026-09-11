using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;
using System.Text;
using System.Text.Json;

namespace PaymentConsumer.Consumers;

public class PaymentConsumer(IConnection connection, ILogger<PaymentConsumer> logger) : BackgroundService
{
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DeclareExchangeAndQueue(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);

            try
            {
                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    await ConsumeAsync(eventArgs, stoppingToken);
                };
            }
            catch (Exception ex)
            {
                logger.LogError("Error occured: {Error}", ex.Message);
            }

            await _channel!.BasicConsumeAsync(
                queue: "payments",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);
        }
    }

    private async Task DeclareExchangeAndQueue(CancellationToken cancellationToken)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: "orders.events",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: "payments",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: "payments",
            exchange: "orders.events",
            routingKey: "order.created",
            cancellationToken: cancellationToken);
    }

    private async Task ConsumeAsync(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        var body = eventArgs.Body.ToArray();
        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(Encoding.UTF8.GetString(body));

        if (orderEvent is null)
        {
            throw new InvalidOperationException($"{nameof(OrderCreatedEvent)} is null");
        }

        logger.LogInformation("Paying order, id: {OrderId}", orderEvent.Id);

        await Task.Delay(Random.Shared.Next(100, 500), cancellationToken);

        await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
