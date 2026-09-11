using HelloRabbitMq.Db;
using HelloRabbitMq.Models;
using RabbitMQ.Client;
using Shared.Events;
using System.Text.Json;

namespace HelloRabbitMq.Producers;

public class OrderProducer(IServiceScopeFactory serviceScopeFactory, IConnection connection) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DeclareExchange(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var order = new Order(Guid.CreateVersion7(), Random.Shared.Next(1000, 5000));

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(stoppingToken);

            await PublishEvent(order, stoppingToken);

            await Task.Delay(Random.Shared.Next(50, 100), stoppingToken);
        }
    }

    private async Task DeclareExchange(CancellationToken cancellationToken)
    {
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: "orders.events",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken);
    }

    private async Task PublishEvent(Order order, CancellationToken cancellationToken)
    {
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(new OrderCreatedEvent(order.Id));

        await channel.BasicPublishAsync(
            exchange: "orders.events",
            routingKey: "order.created",
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body,
            cancellationToken);
    }
}
