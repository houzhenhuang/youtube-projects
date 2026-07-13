namespace eShop.Products.PublicApi;

public class ProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
}
