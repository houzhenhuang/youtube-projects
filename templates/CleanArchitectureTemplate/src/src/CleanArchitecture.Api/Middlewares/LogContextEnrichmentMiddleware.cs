using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace CleanArchitecture.Api.Middlewares;

/// <summary>
/// 表示日志上下文富集中间件
/// </summary>
public class LogContextEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public LogContextEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        using (LogContext.Push(GetEnrichers(httpContext)))
        {
            await _next(httpContext);
        }
    }
    
    /// <summary>
    /// 获取当前请求的 enrichers 数组
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The array of enrichers for the current request.</returns>
    private static ILogEventEnricher[] GetEnrichers(HttpContext httpContext) =>
        [
            new PropertyEnricher("IPAddress", httpContext.Connection.RemoteIpAddress),
            new PropertyEnricher("RequestHost", httpContext.Request.Host),
            new PropertyEnricher("RequestPathBase", httpContext.Request.PathBase),
            new PropertyEnricher("RequestQueryParams", httpContext.Request.QueryString)
        ];
}