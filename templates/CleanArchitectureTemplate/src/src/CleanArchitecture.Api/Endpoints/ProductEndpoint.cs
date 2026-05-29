using CleanArchitecture.Api.Infrastructure;
using CleanArchitecture.Application.Commands.Products;
using CleanArchitecture.Application.Contracts.Products;
using CleanArchitecture.Application.Queries.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.Utility;
using CleanArchitecture.Utility.Primitives.Errors;
using CleanArchitecture.Api.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Endpoints;

/// <summary>
/// 产品端点
/// </summary>
public class ProductEndpoint : EndpointBase
{
    /// <summary>
    /// 
    /// </summary>
    public ProductEndpoint()
        : base("/api/products")
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("", GetProducts).WithName(nameof(GetProducts));

        app.MapGet("/{id:guid}",
            async Task<Results<Ok<ProductResponse>, BadRequest<IReadOnlyCollection<Error>>>> (Guid id, ISender sender) =>
            {
                var query = new GetProductQuery(new ProductId(id));
                
                return await ApiResponse.Create(query, new Error("", ""))
                    .Bind(command => sender.Send(command))
                    .Match(Ok, BadRequest<ProductResponse>);
            });

        app.MapPost("/", async (
            CreateProductRequest request,
            [FromHeader(Name = "X-Idempotency-Key")]
            string requestId,
            ISender sender) =>
        {
            if (!Guid.TryParse(requestId, out var parsedRequestId))
            {
                return BadRequest(new Error("", "RequestId格式不正确"));
            }

            var command = request.Adapt<CreateProductCommand>() with
            {
                RequestId = parsedRequestId
            };

            return await ApiResponse.Create(command, new Error("", ""))
                .Bind(command => sender.Send(command))
                .Match(Ok, BadRequest);
        });

        app.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateProductCommand>() with
            {
                ProductId = new ProductId(id)
            };

            return await ApiResponse.Create(command, new Error("", ""))
                .Bind(command => sender.Send(command))
                .Match(NoContent, NoContentBadRequest);
        });

        app.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            return await ApiResponse.Create(new DeleteProductCommand(new ProductId(id)), new Error("", ""))
                .Bind(command => sender.Send(command))
                .Match(NoContent, NoContentBadRequest);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="sortColum"></param>
    /// <param name="sortOrder"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public async Task<Results<Ok<PagedList<ProductResponse>>, BadRequest<IReadOnlyCollection<Error>>>> GetProducts(
        string? searchTerm,
        string? sortColum,
        string? sortOrder,
        int pageIndex,
        int pageSize,
        ISender sender)
    {
        var query = new GetProductsQuery(searchTerm, sortColum, sortOrder, pageIndex, pageSize);

        return await ApiResponse.Create(query, new Error("", ""))
            .Bind(command => { return sender.Send(command); })
            .Match(Ok, BadRequest<PagedList<ProductResponse>>);
    }
}