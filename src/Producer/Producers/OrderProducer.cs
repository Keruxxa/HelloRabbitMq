using HelloRabbitMq.Db;
using HelloRabbitMq.Models;
using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace HelloRabbitMq.Producers;

public class OrderProducer(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var messageSender = scope.ServiceProvider.GetRequiredService<IMessageSender>();

            var order = new Order(Guid.CreateVersion7(), Random.Shared.Next(1000, 5000));

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(stoppingToken);

            await messageSender.SendAsync(new OrderCreatedEvent(order.Id), stoppingToken);

            await Task.Delay(Random.Shared.Next(50, 100), stoppingToken);
        }
    }
}
