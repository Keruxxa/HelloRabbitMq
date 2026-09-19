using Infrastructure.ServicesRegistration;
using NotificationConsumer.Consumers;
using RabbitMQ.Client;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddRabbitMqConsumer<OrderCreatedEvent, OrderCreatedNotificationConsumer>(options =>
{
    options.Exchange = "orders.events";
    options.Queue = "notifications";
    options.Type = ExchangeType.Topic;
});

builder.Services.AddRabbitMqConsumer<OrderPaidEvent, OrderPaidNotificationConsumer>(options =>
{
    options.Exchange = "orders.events";
    options.Queue = "notifications";
    options.Type = ExchangeType.Topic;
});

var app = builder.Build();

app.Run();