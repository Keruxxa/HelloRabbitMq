using OrderService.Db;
using OrderService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts.Dto;

namespace OrderService.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app
            .MapGroup("api/orders")
            .MapGet("/{id}", GetByIdAsync);
    }

    private static async Task<Results<Ok<OrderDto>, NotFound>> GetByIdAsync(OrderDbContext dbContext, Guid id, CancellationToken cancellationToken)
    {
        var order = await dbContext
            .Orders
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return TypedResults.NotFound();
        }

        var status = order.Status switch
        {
            OrderStatus.Created => "created",
            OrderStatus.Paid => "paid",
            OrderStatus.Shipped => "shipped",
            _ => throw new ArgumentOutOfRangeException(nameof(order.Status))
        };

        return TypedResults.Ok(new OrderDto(order.Id, status, order.Price, order.CreatedAt));
    }
}
