namespace Infrastructure.Messaging.Contracts;

public class RouteInfo(string exchange, string routingKey)
{
    public string Exchange { get; set; } = exchange;
    public string RoutingKey { get; set; } = routingKey;
}
