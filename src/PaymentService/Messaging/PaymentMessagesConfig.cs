using Infrastructure.Messaging.Contracts;
using Shared.Events;

namespace PaymentService.Messaging;

public class PaymentMessagesConfig : IMessagesConfig
{
    private readonly Dictionary<Type, RouteInfo> _routes = new(1)
    {
        { typeof(OrderPaidEvent), new RouteInfo("orders.events", string.Empty) }
    };

    public Dictionary<Type, RouteInfo> Routes => _routes;
}
