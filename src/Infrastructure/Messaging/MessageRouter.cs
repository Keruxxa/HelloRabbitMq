using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging;

public class MessageRouter(IMessagesConfig messagesConfig) : IMessageRouter
{
    public RouteInfo GetRoute<T>(T message)
    {
        if (messagesConfig.Routes.TryGetValue(typeof(T), out var routeInfo))
        {
            return routeInfo;
        }

        throw new ArgumentException($"Type {typeof(T).Name} does not appear in routes");
    }
}
