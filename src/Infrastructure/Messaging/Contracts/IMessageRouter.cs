namespace Infrastructure.Messaging.Contracts;

public interface IMessageRouter
{
    RouteInfo GetRoute<T>(T message);
    void SetMessagesConfig(IMessagesConfig messagesConfig);
}
