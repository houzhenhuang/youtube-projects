using Carter;
using CleanArchitecture.Utility.Primitives.Errors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Api.Endpoints;

/// <summary>
/// minimal api endpoint 基类
/// </summary>
public abstract class EndpointBase : CarterModule
{
    /// <summary>
    /// 
    /// </summary>
    protected EndpointBase()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="basePath"></param>
    protected EndpointBase(string basePath)
        : base(basePath)
    {
    }

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status400BadRequest"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.BadRequest{T}"/>
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    protected Results<Ok, BadRequest<IReadOnlyCollection<Error>>> BadRequest(Error error)
        => BadRequest(new Error[] { error });

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status400BadRequest"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.BadRequest{T}"/>
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    protected Results<Ok, BadRequest<IReadOnlyCollection<Error>>> BadRequest(IReadOnlyCollection<Error> errors)
        => TypedResults.BadRequest(errors);

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status400BadRequest"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.BadRequest{T}"/>
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    protected Results<Ok<TResponse>, BadRequest<IReadOnlyCollection<Error>>> BadRequest<TResponse>(IReadOnlyCollection<Error> errors)
        => TypedResults.BadRequest(errors);

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status400BadRequest"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.BadRequest{T}"/>
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    protected Results<NoContent, BadRequest<IReadOnlyCollection<Error>>> NoContentBadRequest(IReadOnlyCollection<Error> errors)
        => TypedResults.BadRequest(errors);

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status200OK"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.Ok{TValue}"/>
    /// </summary>
    /// <returns></returns>
    protected Results<Ok, BadRequest<IReadOnlyCollection<Error>>> Ok() => TypedResults.Ok();

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status200OK"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.Ok{TValue}"/>
    /// </summary>
    /// <returns> <see cref="Microsoft.AspNetCore.Http.HttpResults.Ok{TValue}"/> </returns>
    protected Results<Ok<TResponse>, BadRequest<IReadOnlyCollection<Error>>> Ok<TResponse>(TResponse? value)
        => TypedResults.Ok(value);

    /// <summary>
    /// 创建一个 <see cref="StatusCodes.Status204NoContent"/> 的 <see cref="Microsoft.AspNetCore.Http.HttpResults.NoContent"/>
    /// </summary>
    /// <returns></returns>
    protected Results<NoContent, BadRequest<IReadOnlyCollection<Error>>> NoContent() => TypedResults.NoContent();
}