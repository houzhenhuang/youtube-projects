using Microsoft.AspNetCore.Authorization;

namespace CustomerManager.Api.Infrastructure.Authorization.Requirements;

/// <summary>
/// 代表权限授权需求。
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 初始化<see cref=“PermissionRequirement”/>类的新实例。
    /// </summary>
    /// <param name="permissionName">The permission name.</param>
    internal PermissionRequirement(string permissionName) => PermissionName = permissionName;

    /// <summary>
    /// 获取权限名称。
    /// </summary>
    internal string PermissionName { get; }
}
