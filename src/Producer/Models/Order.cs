namespace HelloRabbitMq.Models;

public class Order(Guid userId, double price)
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; } = userId;
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public double Price { get; set; } = price;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
