using CleanArchitecture.Application.Commands.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using FluentValidation;

namespace CleanArchitecture.Application.Validations.Products;

/// <summary>
/// 
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="productRepository"></param>
    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("名称不能为空");

        RuleFor(c => c.Name).MustAsync(async (email, _) =>
        {
            return true;
            // return !await customerRepository.IsEmailUniqueAsync(email);
        }).WithMessage("邮箱必须是唯一的");
    }
}