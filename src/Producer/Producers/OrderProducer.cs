using HelloRabbitMq.Db;

namespace HelloRabbitMq.Producers;

public class OrderProducer(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            dbContext.Orders.Add(new(Guid.CreateVersion7(), Random.Shared.Next(1000, 5000)));
            await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(Random.Shared.Next(500, 2000), stoppingToken);
        }
    }
}
