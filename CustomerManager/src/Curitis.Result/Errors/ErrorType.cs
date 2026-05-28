namespace Curitis.Result.Errors;

public enum ErrorType
{
    /// <summary>
    /// 失败
    /// </summary>
    Failure,
    /// <summary>
    /// 验证
    /// </summary>
    Validation,
    /// <summary>
    /// 不存在
    /// </summary>
    NotFound,
    /// <summary>
    /// 冲突,
    /// 比如: 数据库数据已存在
    /// </summary>
    Conflict = 3,
    /// <summary>
    /// 问题
    /// </summary>
    Problem = 4
}