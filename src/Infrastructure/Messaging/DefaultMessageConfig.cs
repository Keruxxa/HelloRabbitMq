using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging;

public class DefaultMessageConfig : IMessagesConfig
{
    public Dictionary<Type, RouteInfo> Routes => [];
}
