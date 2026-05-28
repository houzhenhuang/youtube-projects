using CustomerManager.Api.Infrastructure.Authentication;
using CustomerManager.Api.Infrastructure.Authorization.Enums;
using CustomerManager.Api.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace CustomerManager.Api.Infrastructure.Authorization.Handlers;

/// <summary>
/// 代表权限授权处理程序。
/// </summary>
internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        HashSet<string> permissions = context
            .User
            .Claims
            .Where(x => x.Type == CustomJwtClaimTypes.Permissions)
            .Select(x => x.Value)
            .ToHashSet();

        if (permissions.Any(x => x == requirement.PermissionName || x == Permission.AccessEverything.ToString()))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
