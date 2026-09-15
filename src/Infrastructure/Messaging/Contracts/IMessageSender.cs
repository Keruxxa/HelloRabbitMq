namespace Infrastructure.Messaging.Contracts;

public interface IMessageSender
{
    Task SendAsync<T>(T message, CancellationToken cancellationToken);
}
