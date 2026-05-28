using CustomerManager.Api.Infrastructure.Authorization.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CustomerManager.Api.Infrastructure.Authorization.Attributes;

/// <summary>
/// 表示基于 <see cref="Permission"/> 授权操作的属性。
/// </summary>
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission)
        : base(permission.ToString())
    {

    }
}
