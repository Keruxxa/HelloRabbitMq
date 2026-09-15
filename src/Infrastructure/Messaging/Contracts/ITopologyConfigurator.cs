namespace Infrastructure.Messaging.Contracts;

public interface ITopologyConfigurator
{
    public Task ConfigureAsync(CancellationToken cancellationToken);
}
