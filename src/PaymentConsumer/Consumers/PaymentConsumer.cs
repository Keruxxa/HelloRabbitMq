using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace PaymentConsumer.Consumers;

public class PaymentConsumer(ILogger<PaymentConsumer> logger) : IMessageConsumer<OrderCreatedEvent>
{
    public async Task ConsumeAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Paying order, id: {OrderId}", message.Id);

        await Task.Delay(Random.Shared.Next(100, 500), cancellationToken);
    }
}
