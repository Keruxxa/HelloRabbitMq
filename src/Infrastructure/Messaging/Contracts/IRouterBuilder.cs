namespace Infrastructure.Messaging.Contracts;

public interface IRouterBuilder
{
    void SetMessagesConfig<TConfig>() where TConfig : class, IMessagesConfig;
    void SetTopologyConfigurator<T>() where T : class, ITopologyConfigurator;
}
