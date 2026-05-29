using CleanArchitecture.Api.Infrastructure;
using CleanArchitecture.Application.Primitives.Result;
using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Api.Extensions;

/// <summary>
/// 
/// </summary>
public static class ApiResponseExtensions
{
    /// <summary>
    /// 根据指定的映射函数将结果值映射到新值
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static ApiResponse<TOut> Map<TIn, TOut>(this ApiResponse<TIn> result, Func<TIn, TOut> func) =>
        result.IsSuccess ? func(result.Result!) : ApiResponse.Failure<TOut>(result.Errors);

    /// <summary>
    /// 绑定到函数的结果并返回它
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <returns></returns>
    public static async Task<ApiResponse> Bind<TIn>(this ApiResponse<TIn> result, Func<TIn, Task<Result>> func) =>
        result.IsSuccess ? Map(await func(result.Result!)) : ApiResponse.Failure(result.Errors);

    /// <summary>
    /// 绑定到函数的结果并返回它
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static async Task<ApiResponse<TOut>>
        Bind<TIn, TOut>(this ApiResponse<TIn> result, Func<TIn, Task<Result<TOut>>> func) =>
        result.IsSuccess ? Map(await func(result.Result!)) : ApiResponse.Failure<TOut>(result.Errors);

    /// <summary>
    /// 将结果的成功状态与相应的函数相匹配。
    /// </summary>
    /// <param name="resultTask"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> Match<T>(this Task<ApiResponse> resultTask, Func<T> onSuccess, Func<IReadOnlyCollection<Error>, T> onFailure)
    {
        var result = await resultTask;

        return result.IsSuccess ? onSuccess() : onFailure(result.Errors);
    }

    /// <summary>
    /// 将结果的成功状态与相应的函数相匹配
    /// </summary>
    /// <param name="resultTask"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static async Task<TOut> Match<TIn, TOut>(
        this Task<ApiResponse<TIn>> resultTask,
        Func<TIn?, TOut> onSuccess,
        Func<IReadOnlyCollection<Error>, TOut> onFailure)
    {
        var result = await resultTask;

        return result.IsSuccess ? onSuccess(result.Result) : onFailure(result.Errors);
    }

    private static ApiResponse Map(Result result)
        => new(result.IsSuccess,  result.Errors);

    private static ApiResponse<TOut> Map<TOut>(Result<TOut> result)
        => new(result.Value, result.IsSuccess, result.Errors);
}