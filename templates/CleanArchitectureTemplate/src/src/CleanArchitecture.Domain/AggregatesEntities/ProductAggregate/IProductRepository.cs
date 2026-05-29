namespace CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

public interface IProductRepository
{
    Task<List<Product>> GetList(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task RemoveAsync(Product product, CancellationToken cancellationToken = default);
}