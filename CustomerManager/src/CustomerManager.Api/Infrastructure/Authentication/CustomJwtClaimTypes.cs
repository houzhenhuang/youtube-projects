namespace CustomerManager.Api.Infrastructure.Authentication;

/// <summary>
/// 包含应用程序中使用的自定义 JWT 声明类型。
/// </summary>
public class CustomJwtClaimTypes
{
    /// <summary>
    /// 全名称 claim type.
    /// </summary>
    public const string FullName = "full_name";

    /// <summary>
    /// 权限 claim type.
    /// </summary>
    public const string Permissions = "permissions";
}
