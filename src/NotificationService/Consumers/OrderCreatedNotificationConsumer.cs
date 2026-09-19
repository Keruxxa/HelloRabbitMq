using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace NotificationService.Consumers;

public class OrderCreatedNotificationConsumer(ILogger<OrderCreatedNotificationConsumer> logger) : IMessageConsumer<OrderCreatedEvent>
{
    public async Task ConsumeAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        // Sending an email
        logger.LogInformation("Order was created successfully. Id: {OrderId}", message.Id);

        await Task.Delay(Random.Shared.Next(30, 50), cancellationToken);
    }
}
