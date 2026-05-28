namespace Orders.Processing;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalWeight { get; set; }
}