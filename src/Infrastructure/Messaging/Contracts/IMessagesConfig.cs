namespace Infrastructure.Messaging.Contracts;

public interface IMessagesConfig
{
    public Dictionary<Type, RouteInfo> Routes { get; }
}
