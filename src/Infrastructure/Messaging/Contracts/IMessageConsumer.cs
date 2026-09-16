namespace Infrastructure.Messaging.Contracts;

public interface IMessageConsumer<in TMessage>
{
    Task ConsumeAsync(TMessage message, CancellationToken cancellationToken);
}
