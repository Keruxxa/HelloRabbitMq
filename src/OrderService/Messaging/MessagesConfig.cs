using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace HelloRabbitMq.Messaging;

public class MessagesConfig : IMessagesConfig
{
    public Dictionary<Type, RouteInfo> Routes => new(1)
    {
        { typeof(OrderCreatedEvent), new RouteInfo("orders.events", "order.created") }
    };
}
