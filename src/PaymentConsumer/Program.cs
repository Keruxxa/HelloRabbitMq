using Infrastructure.ServicesRegistration;
using RabbitMQ.Client;
using Shared.Events;
using PaymentConsumerService = PaymentConsumer.Consumers.PaymentConsumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddRabbitMqConsumer<OrderCreatedEvent, PaymentConsumerService>(options =>
{
    options.Queue = "payments";
    options.Exchange = "orders.events";
    options.RoutingKey = "order.created";
    options.Type = ExchangeType.Topic;
});

var app = builder.Build();

app.Run();
