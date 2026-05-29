using CleanArchitecture.Application.Commands.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Application.CommandsHandlers.Products;

/// <summary>
/// 修改产品处理器
/// </summary>
internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        if (product is null)
        {
            return Result.Failure(new Error("Product.NotFount", "产品不存在"));
        }
        
        product.Update(request.Name, new Money(request.Currency, request.Amount), Sku.Create(request.Sku)!);
        
        await _productRepository.UpdateAsync(product, cancellationToken);
        
        return Result.Success();
    }
}