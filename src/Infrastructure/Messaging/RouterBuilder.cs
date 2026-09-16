using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging;

public class RouterBuilder
{
    /// <summary>
    ///     Defines exchanges, queues and binds them
    /// </summary>
    public Type TopologyConfigurator { get; set; } = typeof(ITopologyConfigurator);

    /// <summary>
    ///     Maps event types to their routes
    /// </summary>
    public IMessagesConfig MessagesConfig { get; set; } = new DefaultMessageConfig();
}
