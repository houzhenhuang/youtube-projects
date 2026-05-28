using Curitis.Result;
using Curitis.Result.Extensions;
using Curitis.Result.FluentValidation;
using CustomerManager.Api.Endpoints;
using CustomerManager.Api.Features.Products.Entities;
using FluentValidation;
using MediatR;
using OpenApiSamples.Data;

namespace CustomerManager.Api.Features.Products;

file record Request(string Name, decimal Price);
file record Response(int Id, string Name, decimal Price);
file sealed record CreateCommand(string Name, decimal Price) : IRequest<Result<int>>;
file sealed class Validator : AbstractValidator<CreateCommand>
{
    public Validator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Price).GreaterThan(0);
    }
}
file sealed class CreateCommandHandler : IRequestHandler<CreateCommand, Result<int>>
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateCommand> _validator;

    public CreateCommandHandler(AppDbContext dbContext, IValidator<CreateCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }
    public async Task<Result<int>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<int>(validationResult.Errors.ToArray().CreateValidationError());
        }

        Product product = new() { Name = request.Name, Price = request.Price };

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        return Result.Success(product.Id);
    }
}

file static class CreateProduct
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("products", Handler).WithTags("Products");
        }

        public static async Task<IResult> Handler(Request request, ISender sender)
        {
            var command = new CreateCommand(request.Name, request.Price);
            Result<int> result = await sender.Send(command);

            return result.Match(value => Results.Ok(value), error => Results.BadRequest(error));
        }
    }
}
