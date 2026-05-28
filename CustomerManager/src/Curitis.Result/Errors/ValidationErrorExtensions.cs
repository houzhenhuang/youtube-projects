namespace Curitis.Result.Errors;

public static class ValidationErrorExtensions
{
    /// <summary>
    /// 验证失败转验证失败结果
    /// </summary>
    /// <param name="validationError"></param>
    /// <returns></returns>
    public static Result<TValue> ToResult<TValue>(this ValidationError validationError)
       => Result.Failure<TValue>(validationError);
}