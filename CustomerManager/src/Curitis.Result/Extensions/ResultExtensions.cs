using Curitis.Result.Errors;

namespace Curitis.Result.Extensions;

public static class ResultExtensions
{
    public static TResult Match<TResult>(this Result result, Func<TResult> success, Func<TResult> failure)
         => result.IsSuccess ? success.Invoke() : failure.Invoke();

    public static TResult Match<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> success, Func<Error, TResult> failure)
        => result.IsSuccess ? success.Invoke(result.Value!) : failure.Invoke(result.Error);
}
