using System.Net;
using System.Text.Json;
using CleanArchitecture.Api.Constants;
using CleanArchitecture.Application.Abstractions.Exceptions;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Utility.Primitives.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Middlewares;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionHandlerV2Middleware
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
    public GlobalExceptionHandlerV2Middleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
    /// <param name="httpContext">The HTTP httpContext.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The HTTP response that is modified based on the exception.</returns>
    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var exceptionDetails = GetExceptionDetails(exception);

        var problemDetail = new ProblemDetails
        {
            Status = exceptionDetails.Status,
            Type = exceptionDetails.Type,
            Title = exceptionDetails.Title,
            Detail = exceptionDetails.Detail,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        if (exceptionDetails.Errors is not null)
        {
            problemDetail.Extensions["errors"] = exceptionDetails.Errors;
        }

        httpContext.Response.StatusCode = exceptionDetails.Status;

        await httpContext.Response.WriteAsJsonAsync(problemDetail);
    }


    private ExceptionDetails GetExceptionDetails(Exception ex)
    {
        return ex switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                ExceptionConst.ValidationFailureType,
                ExceptionConst.ValidationFailureTitle,
                ExceptionConst.ValidationFailureDetail,
                validationException.Errors
            ),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                ExceptionConst.ServerErrorType,
                ExceptionConst.ServerErrorTitle,
                ExceptionConst.ServerErrorDetail,
                null)
        };
    }
}

internal record ExceptionDetails(
    int Status,
    string Type,
    string Title,
    string Detail,
    IEnumerable<object>? Errors);