namespace Curitis.Result.Errors;

/// <summary>
/// 错误类
/// </summary>
public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(ErrorConst.NullValueCode, "This is an null value.", ErrorType.Failure);

    protected Error(string code, string message, ErrorType errorType)
    {
        Message = message;
        Code = code;
        Type = errorType;
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 错误码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 错误类型
    /// </summary>
    public ErrorType Type { get; set; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);
}
