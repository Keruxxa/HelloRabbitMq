using Infrastructure.ServicesRegistration;
using PaymentConsumer.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMq(builder.Configuration, options =>
{
    options.TopologyConfigurator = typeof(TopologyConfigurator);
});

builder.Services.AddHostedService<PaymentConsumer.Consumers.PaymentConsumer>();

var app = builder.Build();

app.Run();