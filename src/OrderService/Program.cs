using OrderService.Db;
using OrderService.Endpoints;
using OrderService.Messaging;
using OrderService.Producers;
using Infrastructure.ServicesRegistration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddRabbitMq(builder.Configuration, routerBuilder =>
{
    routerBuilder.SetMessagesConfig<MessagesConfig>();
    routerBuilder.SetTopologyConfigurator<TopologyConfigurator>();
});

builder.Services.AddHostedService<OrderProducer>();

var app = builder.Build();

app.MapOrderEndpoints();

app.Run();
