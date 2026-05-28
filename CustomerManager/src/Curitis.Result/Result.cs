using Curitis.Result.Errors;

namespace Curitis.Result;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
           !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error.", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 是否失败
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// 成功消息
    /// </summary>
    public string SuccessMessage { get; protected set; } = string.Empty;

    /// <summary>
    /// 错误
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// 成功的结果
    /// </summary>
    /// <returns></returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// 失败的结果
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// 带返回值成功的结果
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// 带返回值失败的结果
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}


