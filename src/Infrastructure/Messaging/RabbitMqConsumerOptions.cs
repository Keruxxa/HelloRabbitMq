using RabbitMQ.Client;

namespace Infrastructure.Messaging;

public class RabbitMqConsumerOptions<TMessage>
{
    public string Queue { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public string Type { get; set; } = ExchangeType.Fanout;
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; }
    public bool AutoAck { get; set; }
    public bool RequeueOnFailure { get; set; }
    public ushort PrefetchCount { get; set; } = 1;
    public IDictionary<string, object?>? QueueArguments { get; set; }
    public IDictionary<string, object?>? ExchangeArguments { get; set; }
}
