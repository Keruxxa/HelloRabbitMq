using Infrastructure.ServicesRegistration;
using PaymentService.Consumers;
using PaymentService.Messaging;
using RabbitMQ.Client;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMq(builder.Configuration, routerBuilder =>
{
    routerBuilder.SetMessagesConfig<PaymentMessagesConfig>();
    routerBuilder.SetTopologyConfigurator<PaymentTopologyConfigurator>();
});

builder.Services.AddRabbitMqConsumer<OrderCreatedEvent, PaymentConsumer>(options =>
{
    options.Exchange = "orders.events";
    options.Queue = "payments";
    options.RoutingKey = "order.created";
    options.Type = ExchangeType.Topic;
});

var app = builder.Build();

app.Run();
