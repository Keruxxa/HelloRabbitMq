using OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Db;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
}
