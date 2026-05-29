namespace CleanArchitecture.Utility.Primitives.Errors;

/// <summary>
/// 错误消息实体
/// </summary>
public sealed class Error : IEquatable<Error>
{
    /// <summary>
    /// 初始化 <see cref="Error"/> 实例
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误消息</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// 获取错误码
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// 获取错误消息
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 自定义隐式转换
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator string(Error? error) => error?.Code ?? string.Empty;

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b) => !(a == b);

    /// <inheritdoc />
    public bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not Error error)
        {
            return false;
        }

        return Equals(error);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Code, Message);
}