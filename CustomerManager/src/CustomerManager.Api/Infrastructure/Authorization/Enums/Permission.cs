namespace CustomerManager.Api.Infrastructure.Authorization.Enums;

/// <summary>
/// 表示权限枚举。
/// </summary>
public enum Permission
{
    /// <summary>
    /// 默认权限值。
    /// </summary>
    None = 0,

    /// <summary>
    /// 用户读取权限。
    /// </summary>
    UserRead = 1,

    /// <summary>
    /// 用户修改权限。
    /// </summary>
    UserModify = 2,

    /// <summary>
    /// 访问一切权限。
    /// </summary>
    AccessEverything = int.MaxValue
}
