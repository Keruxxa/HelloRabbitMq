using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace PaymentConsumer.Consumers;

public class PaymentConsumer(IMessageSender messageSender, ILogger<PaymentConsumer> logger) : IMessageConsumer<OrderCreatedEvent>
{
    public async Task ConsumeAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Order was paid successfully. Id: {OrderId}", message.Id);

        await Task.Delay(Random.Shared.Next(100, 500), cancellationToken);

        await messageSender.SendAsync(new OrderPaidEvent(message.Id), cancellationToken);
    }
}
