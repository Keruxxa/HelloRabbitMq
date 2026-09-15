using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging;

public class RouterBuilder
{
    public Type TopologyConfigurator { get; set; } = typeof(DefaultMessageConfig);
    public IMessagesConfig MessagesConfig { get; set; } = new DefaultMessageConfig();
}
