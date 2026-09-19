namespace Infrastructure.Messaging.Contracts;

public interface IMessageRouter
{
    RouteInfo GetRoute<T>(T message);
}
