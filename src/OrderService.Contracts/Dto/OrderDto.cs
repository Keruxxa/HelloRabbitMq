namespace OrderService.Contracts.Dto;

public record OrderDto(Guid Id, string Status, double Price, DateTime CreatedAt);
