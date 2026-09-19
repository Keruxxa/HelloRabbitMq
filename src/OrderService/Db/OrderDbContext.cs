using HelloRabbitMq.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloRabbitMq.Db;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
}
