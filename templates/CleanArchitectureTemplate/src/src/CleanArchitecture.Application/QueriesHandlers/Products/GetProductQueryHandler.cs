using CleanArchitecture.Application.Contracts.Products;
using CleanArchitecture.Application.Queries.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Application.QueriesHandlers.Products;

internal sealed class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductResponse>> Handle(GetProductQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(new Error("Product.NotFound", "产品不存在"));
        }

        return new ProductResponse(product.Id.Value, product.Name, (string)product.Sku, product.Price.Currency,
            product.Price.Amount);
    }
}