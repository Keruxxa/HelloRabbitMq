using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace NotificationService.Consumers;

public class OrderPaidNotificationConsumer(ILogger<OrderPaidNotificationConsumer> logger) : IMessageConsumer<OrderPaidEvent>
{
    public async Task ConsumeAsync(OrderPaidEvent message, CancellationToken cancellationToken)
    {
        // Sending an email
        logger.LogInformation("Order was paid successfully. Id: {OrderId}", message.Id);

        await Task.Delay(Random.Shared.Next(30, 50), cancellationToken);
    }
}
