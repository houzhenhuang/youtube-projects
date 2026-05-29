using System.Linq.Expressions;
using CleanArchitecture.Application.Abstractions.Caching;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Contracts.Products;
using CleanArchitecture.Application.Queries.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.Utility;

namespace CleanArchitecture.Application.QueriesHandlers.Products;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedList<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IApplicationDbContext _dbContext;

    public GetProductsQueryHandler(IProductRepository productRepository,
        ICacheService cacheService,
        IApplicationDbContext dbContext)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<Result<PagedList<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var cachePrefixKey = $"products:{request.PageIndex}:{request.PageSize}";
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            cachePrefixKey += $":{request.SearchTerm}";
        }

        if (!string.IsNullOrWhiteSpace(request.SortColum))
        {
            cachePrefixKey += $":{request.SortColum}";
        }

        if (!string.IsNullOrWhiteSpace(request.SortOrder))
        {
            cachePrefixKey += $":{request.SortOrder}";
        }

        var products = await _cacheService.GetAsync(cachePrefixKey, async () =>
        {
            IQueryable<Product> productsQuery = _dbContext.Products;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(request.SearchTerm) || ((string)p.Sku).Contains(request.SearchTerm));
            }

            var keySelector = GetSortProperty(request);

            productsQuery = request.SortOrder?.ToLower() == "desc"
                ? productsQuery.OrderByDescending(keySelector)
                : productsQuery.OrderBy(keySelector);

            var productResponseQuery = productsQuery
                .Select(p => new ProductResponse(
                    p.Id.Value,
                    p.Name,
                    (string)p.Sku,
                    p.Price.Currency,
                    p.Price.Amount));

            return await PagedList<ProductResponse>.CreateAsync(
                productResponseQuery,
                request.PageIndex,
                request.PageSize);
        }, cancellationToken);

        return Result.Success(products);
    }

    private Expression<Func<Product, object>> GetSortProperty(GetProductsQuery request) =>
        request.SortColum?.ToLower() switch
        {
            "name" => product => product.Name,
            "sku" => product => product.Sku,
            "amount" => product => product.Price.Amount,
            "currency" => product => product.Price.Currency,
            _ => product => product.Id,
        };
}