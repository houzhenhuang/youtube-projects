namespace CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

public class Product : AggregateRoot<ProductId>, IAuditableEntity
{
    public Product()
    {
        Id = new ProductId(Guid.Empty);
        Price = new Money("", 0);
    }

    public Product(ProductId id, string name, Money price, Sku sku)
        : base(id)
    {
        Name = name;
        Price = price;
        Sku = sku;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public string Name { get; private set; } = string.Empty;

    public Money Price { get; private set; }

    public Sku? Sku { get; private set; }

    public DateTime CreatedOnUtc { get; }

    public DateTime? ModifiedOnUtc { get; private set; }

    public void Update(string name, Money price, Sku sku)
    {
        Name = name;
        Price = price;
        Sku = sku;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}