namespace eShop.Orders.Api.Models;

public class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }  // Foreign key

    public int ProductId { get; set; }

    public required string ProductName { get; set; }

    public decimal UnitPrice { get; set; }  // Denormalized
    public int Quantity { get; set; }

    public Order? Order { get; set; } // Navigation property
}