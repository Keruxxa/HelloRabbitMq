using Infrastructure.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Infrastructure.Messaging;

public sealed class RabbitMqConsumerHostedService<TMessage>(
    IChannelPool channelPool,
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqConsumerOptions<TMessage>> options,
    ILogger<RabbitMqConsumerHostedService<TMessage>> logger) : BackgroundService
{
    private IChannel? _channel;
    private string? _consumerTag;
    private readonly object _inFlightLock = new();
    private int _inFlight;
    private TaskCompletionSource _inFlightEmpty = new(TaskCreationOptions.RunContinuationsAsynchronously);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerOptions = options.Value;

        _channel = await channelPool.GetChannelAsync(stoppingToken);

        try
        {
            await DeclareTopologyAsync(consumerOptions, _channel, stoppingToken);
            await _channel.BasicQosAsync(0, consumerOptions.PrefetchCount, false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (_, eventArgs) => HandleAsync(eventArgs, consumerOptions, stoppingToken);

            _consumerTag = await _channel.BasicConsumeAsync(
                queue: consumerOptions.Queue,
                autoAck: consumerOptions.AutoAck,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        finally
        {
            await ReturnChannelAsync();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null && _consumerTag is not null)
        {
            try
            {
                await _channel.BasicCancelAsync(_consumerTag, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to cancel consumer on queue {Queue}", options.Value.Queue);
            }
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task DeclareTopologyAsync(
        RabbitMqConsumerOptions<TMessage> consumerOptions,
        IChannel channel,
        CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: consumerOptions.Exchange,
            type: consumerOptions.Type,
            durable: consumerOptions.Durable,
            autoDelete: consumerOptions.AutoDelete,
            arguments: consumerOptions.ExchangeArguments,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: consumerOptions.Queue,
            durable: consumerOptions.Durable,
            exclusive: false,
            autoDelete: consumerOptions.AutoDelete,
            arguments: consumerOptions.QueueArguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: consumerOptions.Queue,
            exchange: consumerOptions.Exchange,
            routingKey: consumerOptions.RoutingKey,
            cancellationToken: cancellationToken);
    }

    private async Task HandleAsync(
        BasicDeliverEventArgs eventArgs,
        RabbitMqConsumerOptions<TMessage> consumerOptions,
        CancellationToken cancellationToken)
    {
        StartMessage();

        try
        {
            await HandleCoreAsync(eventArgs, consumerOptions, cancellationToken);
        }
        finally
        {
            CompleteMessage();
        }
    }

    private async Task HandleCoreAsync(
        BasicDeliverEventArgs eventArgs,
        RabbitMqConsumerOptions<TMessage> consumerOptions,
        CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Deserialize<TMessage>(eventArgs.Body.Span);

            if (message is null)
            {
                throw new InvalidOperationException($"Failed to deserialize message {typeof(TMessage).Name} from queue {consumerOptions.Queue}");
            }

            using var scope = scopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer<TMessage>>();

            await consumer.ConsumeAsync(message, cancellationToken);

            if (!consumerOptions.AutoAck)
            {
                await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while consuming message from queue {Queue}", consumerOptions.Queue);

            if (!consumerOptions.AutoAck)
            {
                await TryNackAsync(eventArgs.DeliveryTag, consumerOptions.RequeueOnFailure, cancellationToken);
            }
        }
    }

    private void StartMessage()
    {
        lock (_inFlightLock)
        {
            _inFlight++;
        }
    }

    private void CompleteMessage()
    {
        lock (_inFlightLock)
        {
            _inFlight--;

            if (_inFlight == 0)
            {
                _inFlightEmpty.TrySetResult();
            }
        }
    }

    private Task WaitForInFlightMessagesAsync()
    {
        lock (_inFlightLock)
        {
            if (_inFlight == 0)
            {
                return Task.CompletedTask;
            }

            if (_inFlightEmpty.Task.IsCompleted)
            {
                _inFlightEmpty = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            return _inFlightEmpty.Task;
        }
    }

    private async Task TryNackAsync(ulong deliveryTag, bool requeue, CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            await _channel.BasicNackAsync(deliveryTag, false, requeue, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to nack message {DeliveryTag}", deliveryTag);
        }
    }

    private async Task ReturnChannelAsync()
    {
        await WaitForInFlightMessagesAsync();

        if (_channel is not null)
        {
            await channelPool.Return(_channel);
            _channel = null;
            _consumerTag = null;
        }
    }
}
