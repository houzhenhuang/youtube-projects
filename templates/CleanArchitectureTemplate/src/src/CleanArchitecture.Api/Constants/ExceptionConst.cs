namespace CleanArchitecture.Api.Constants;

public class ExceptionConst
{
    public const string ValidationFailureType = "ValidationFailure";
    public const string ValidationFailureTitle = "Validation error";
    public const string ValidationFailureDetail = "出现了一个或多个验证错误";
    
    
    public const string ServerErrorType = "ServerError";
    public const string ServerErrorTitle = "Server error";
    public const string ServerErrorDetail = "服务器遇到不可恢复的错误";
}