using Infrastructure.ServicesRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddHostedService<PaymentConsumer.Consumers.PaymentConsumer>();

var app = builder.Build();

app.Run();