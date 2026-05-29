using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Api.Infrastructure;

/// <summary>
/// api 响应结果
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="errors"></param>
    public ApiResponse(bool isSuccess, IReadOnlyCollection<Error> errors)
    {
        IsSuccess = isSuccess && !errors.Any();
        Errors = errors;
    }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 获取错误信息
    /// </summary>
    public IReadOnlyCollection<Error> Errors { get; }

    /// <summary>
    /// 使用指定的可为 null 的值和指定的错误创建一个新的 <see cref="ApiResponse{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="result">结果值</param>
    /// <returns> 指定值或错误的 <see cref="ApiResponse{TResult}"/> 的新实例</returns>
    public static ApiResponse<TResult> Create<TResult>(TResult result)
        where TResult : class
        => result is null ? Failure<TResult>(new Error[] { new Error("ReuqestParamter.NotNull","请求参数不能为空") }) : Success(result);

    /// <summary>
    /// 使用指定的可为 null 的值和指定的错误创建一个新的 <see cref="ApiResponse{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="result">结果值</param>
    /// <param name="error">值为null时的错误</param>
    /// <returns> 指定值或错误的 <see cref="ApiResponse{TResult}"/> 的新实例</returns>
    public static ApiResponse<TResult> Create<TResult>(TResult result, Error error)
        where TResult : class
        => result is null ? Failure<TResult>(new Error[] { error }) : Success(result);

    /// <summary>
    /// 返回成功 <see cref="ApiResponse"/>
    /// </summary>
    /// <returns></returns>
    public static ApiResponse Success() => new(true, new Error[] { });

    /// <summary>
    /// 返回具有指定值的成功结果 <see cref="ApiResponse{TResult}"/>
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static ApiResponse<TResult> Success<TResult>(TResult result) => new(result, true, null);

    /// <summary>
    /// 返回具有指定错误的结果 <see cref="ApiResponse{TResult}"/>
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ApiResponse Failure(Error error) => Failure(new[] { error });

    /// <summary>
    /// 返回具有指定错误的结果 <see cref="ApiResponse{TResult}"/>
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static ApiResponse Failure(IReadOnlyCollection<Error> errors) => new(false, errors);

    /// <summary>
    /// 返回具有指定错误的结果 <see cref="ApiResponse{TResult}"/>
    /// </summary>
    /// <param name="errors"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static ApiResponse<TResult> Failure<TResult>(IReadOnlyCollection<Error> errors) => new(default, false, errors);
}