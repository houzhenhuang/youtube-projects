namespace Curitis.Result.Errors;

public sealed record ValidationError : Error
{
    private ValidationError(Error[] errors)
       : base(ErrorConst.ValidationErrorCode, "出现一个或多个验证错误", ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    /// <summary>
    /// 将结果中失败的 <see cref="Result.Error"/> 转 <see cref="ValidationError"/>
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
