using CleanArchitecture.Application.Commands.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

namespace CleanArchitecture.Application.CommandsHandlers.Products;

/// <summary>
/// 创建产品处理器
/// </summary>
internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            new ProductId(Guid.NewGuid()),
            request.Name,
            new Money(request.Currency, request.Amount),
            Sku.Create(request.Sku)!
        );

        await _productRepository.AddAsync(product, cancellationToken);

        return Result.Success();
    }
}