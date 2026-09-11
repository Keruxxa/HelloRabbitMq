using HelloRabbitMq.Db;
using HelloRabbitMq.Producers;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ServicesRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddHostedService<OrderProducer>();

var app = builder.Build();

app.Run();
