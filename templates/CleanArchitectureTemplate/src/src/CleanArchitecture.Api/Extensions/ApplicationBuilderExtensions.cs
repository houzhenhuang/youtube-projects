using CleanArchitecture.Api.Middlewares;

namespace CleanArchitecture.Api.Extensions;

/// <summary>
/// 
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 配置全局异常处理程序中间件
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The same application builder.</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        => builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    
    /// <summary>
    /// 配置日志上下文丰富中间件
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The same application builder.</returns>
    public static IApplicationBuilder UseLogContextEnrichment(this IApplicationBuilder builder)
        => builder.UseMiddleware<LogContextEnrichmentMiddleware>();
}