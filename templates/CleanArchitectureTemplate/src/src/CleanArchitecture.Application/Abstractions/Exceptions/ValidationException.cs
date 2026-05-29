using CleanArchitecture.Utility.Primitives.Errors;
using FluentValidation.Results;

namespace CleanArchitecture.Application.Abstractions.Exceptions;

/// <summary>
/// 表示验证失败时发生的异常
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// 初始化 <see cref="ValidationException"/> 类
    /// </summary>
    /// <param name="failures">验证失败的集合</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures has occurred.")
        => Errors = failures
            .Distinct()
            .Select(failure => new Error(failure.ErrorCode, failure.ErrorMessage))
            .ToArray();

    /// <summary>
    /// 获取错误集合
    /// </summary>
    public IReadOnlyCollection<Error> Errors { get; }
}