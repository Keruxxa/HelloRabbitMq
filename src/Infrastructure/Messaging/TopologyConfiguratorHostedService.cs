using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Messaging;

public class TopologyConfiguratorHostedService(ITopologyConfigurator topologyConfigurator) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await topologyConfigurator.ConfigureAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
