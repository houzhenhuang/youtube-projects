using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.EntityFrameworkCore.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetList(CancellationToken cancellationToken = default)
    {
    
        // await _dbContext.Products.
        return null;
    }

    public async Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }


    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(product.Id, cancellationToken);
        if (entity is null)
        {
        }
    }

    public async Task RemoveAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(product);

        await Task.CompletedTask;
    }
}