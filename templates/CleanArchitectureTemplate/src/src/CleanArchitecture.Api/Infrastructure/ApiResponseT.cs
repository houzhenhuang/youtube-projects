using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Api.Infrastructure;

/// <summary>
/// api 响应结构
/// </summary>
/// <typeparam name="TResult"></typeparam>
public class ApiResponse<TResult> : ApiResponse
{
    private readonly TResult? _result;

    /// <summary>
    /// 使用指定的参数初始化 <see cref="ApiResponse{TResult}"/> 类的新实例
    /// </summary>
    /// <param name="result">结果值</param>
    /// <param name="isSuccess">指示结果是否成功的标志</param>
    /// <param name="errors">错误</param>
    public ApiResponse(TResult? result, bool isSuccess, IReadOnlyCollection<Error> errors)
        : base(isSuccess, errors == default ? Array.Empty<Error>() : errors)
        => _result = result;

    /// <summary>
    /// 如果结果成功,则获取结果值,否则引发异常
    /// </summary>
    /// <returns>如果结果成功，则为结果值</returns>
    /// <exception cref="InvalidOperationException"> 当 <see cref="ApiResponse.IsSuccess"/> 为 False 时</exception>
    public TResult? Result => IsSuccess
        ? _result
        : throw new InvalidOperationException("无法访问失败结果的值。");

    /// <summary>
    /// 隐式转换
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ApiResponse<TResult>(TResult value) => Success(value);
}