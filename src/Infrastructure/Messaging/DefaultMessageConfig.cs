using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging;

public class DefaultMessageConfig : IMessagesConfig
{
    private readonly Dictionary<Type, RouteInfo> _routes = [];

    public Dictionary<Type, RouteInfo> Routes => _routes;
}
