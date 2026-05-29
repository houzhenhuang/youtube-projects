using System.Reflection;

namespace CleanArchitecture.Application.Validations;

/// <summary>
/// 验证器所在程序集
/// </summary>
public static class ValidationsAssembly
{
    /// <summary>
    /// 获取验证器程序程序集
    /// </summary>
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
}