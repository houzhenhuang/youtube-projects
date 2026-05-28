using Curitis.Result;
using Curitis.Result.Errors;
using Curitis.Result.FluentValidation;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Reflection;

namespace CustomerManager.Api.Abstractions.Behaviors;

internal sealed class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidatorPipelineBehavior<TRequest, TResponse>> _logger;

    public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(request);

        if (validationFailures.Length == 0)
        {
            return await next();
        }

        //_logger.LogError($"验证失败：${JsonSerializer.Serialize(validationFailures)}");
        _logger.LogError("验证失败:{$validationFailures}", validationFailures);

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type result = typeof(TResponse).GetGenericArguments()[0];

            MethodInfo failureMethod = typeof(ValidationErrorExtensions)
                .GetMethod(nameof(ValidationErrorExtensions.ToResult))!
                .MakeGenericMethod(result);

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, [validationFailures.CreateValidationError()])!;
            }
        }

        return (TResponse)Result.Failure(validationFailures.CreateValidationError());
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (!_validators.Any())
        {
            return Array.Empty<ValidationFailure>();
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }
}
