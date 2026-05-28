using Curitis.Result.Errors;

namespace Curitis.Result;

public class Result<TValue> : Result
{
    private readonly TValue? _value;
    protected internal Result(TValue? value, bool isSuccess, Error error)
           : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue? Value => IsSuccess ? _value : throw new InvalidOperationException("The result of failure cannot access this value.");

    /// <summary>
    /// 隐式转换运算符
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator TValue?(Result<TValue> result) => result.Value;

    /// <summary>
    /// 隐式转换运算符
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Result<TValue>(TValue value)
        => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
