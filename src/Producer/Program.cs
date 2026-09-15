using HelloRabbitMq.Db;
using HelloRabbitMq.Messaging;
using HelloRabbitMq.Producers;
using Infrastructure.ServicesRegistration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddRabbitMq(builder.Configuration, options =>
{
    options.MessagesConfig = new MessagesConfig();
    options.TopologyConfigurator = typeof(TopologyConfigurator);
});

builder.Services.AddHostedService<OrderProducer>();

var app = builder.Build();

app.Run();
