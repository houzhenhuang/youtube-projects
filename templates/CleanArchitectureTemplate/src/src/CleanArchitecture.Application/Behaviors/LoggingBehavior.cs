using CleanArchitecture.Application.Extensions;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Behaviors;

/// <summary>
/// 表示日志记录行为中间件
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----- Handling command {CommandName} ({@Command})", request.GetGenericTypeName(), request);

        var response = await next();

        if (response.IsFailure)
        {
            _logger.LogError("----- Handling command {CommandName} Failure ({@Command}), {@Error}",
                request.GetGenericTypeName(), request, response.Errors);
        }

        _logger.LogInformation("----- Command {CommandName} handled - response: {@Response}",
            request.GetGenericTypeName(), response);

        return response;
    }
}