using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging;

public class MessageRouter : IMessageRouter
{
    private IMessagesConfig _messagesConfig = new DefaultMessageConfig();

    public RouteInfo GetRoute<T>(T message)
    {
        if (_messagesConfig.Routes.TryGetValue(typeof(T), out var routeInfo))
        {
            return routeInfo;
        }

        throw new ArgumentException($"Type {typeof(T).Name} does not appear in routes");
    }

    public void SetMessagesConfig(IMessagesConfig messagesConfig)
    {
        _messagesConfig = messagesConfig;
    }
}
