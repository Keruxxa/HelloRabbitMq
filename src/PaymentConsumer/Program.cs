using Infrastructure.ServicesRegistration;
using PaymentConsumer.Messaging;
using RabbitMQ.Client;
using Shared.Events;
using PaymentConsumerService = PaymentConsumer.Consumers.PaymentConsumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMq(builder.Configuration, routerBuilder =>
{
    routerBuilder.MessagesConfig = new PaymentMessagesConfig();
    routerBuilder.TopologyConfigurator = typeof(PaymentTopologyConfigurator);
});

builder.Services.AddRabbitMqConsumer<OrderCreatedEvent, PaymentConsumerService>(options =>
{
    options.Exchange = "orders.events";
    options.Queue = "payments";
    options.RoutingKey = "order.created";
    options.Type = ExchangeType.Topic;
});

var app = builder.Build();

app.Run();
