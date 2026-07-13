namespace eShop.Orders.Api.Models;

public class Order
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public DateTime OrderDate { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}
