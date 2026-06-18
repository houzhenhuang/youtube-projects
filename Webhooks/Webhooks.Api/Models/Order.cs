namespace Webhooks.Api.Models;

public class Order
{
    public Order()
    {
        
    }
    public Order(Guid id, string customerName, decimal amount, DateTime createAt)
    {
        Id = id;
        CustomerName = customerName;
        Amount = amount;
        CreatedAt = createAt;
    }
    public Guid Id { get; set; }

    public string CustomerName { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed record CreateOrderRequest(string CustomerName, decimal Amount);