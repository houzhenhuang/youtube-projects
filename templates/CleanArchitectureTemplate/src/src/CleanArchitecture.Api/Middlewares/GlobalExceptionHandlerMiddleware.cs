using System.Net;
using System.Text.Json;
using CleanArchitecture.Application.Abstractions.Exceptions;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Api.Middlewares;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// Handles the specified <see cref="Exception"/> for the specified <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The HTTP httpContext.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The HTTP response that is modified based on the exception.</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error> errors) = GetHttpStatusCodeAndErrors(exception);

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = (int)httpStatusCode;

        string response = JsonSerializer.Serialize(errors, JsonSerializerOptions);

        await context.Response.WriteAsync(response);
    }

    /// <summary>
    /// Gets the HTTP status code and collection of errors for the specified exception.
    /// </summary>
    /// <param name="exception">The exception that has occurred.</param>
    /// <returns>The HTTP status code and collection of errors for the specified exception.</returns>
    private static (HttpStatusCode StatusCode, IReadOnlyCollection<Error> Errors) GetHttpStatusCodeAndErrors(
        Exception exception) =>
        exception switch
        {
            ValidationException validationException => (HttpStatusCode.BadRequest, validationException.Errors),
            DomainException domainException => (HttpStatusCode.UnprocessableEntity, new[] { domainException.Error }),
            _ => (HttpStatusCode.InternalServerError,
                new[] { new Error("API.ServerError", "服务器遇到不可恢复的错误") })
        };
}