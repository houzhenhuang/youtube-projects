using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CustomerManager.Api.Infrastructure.Authentication;
using CustomerManager.Api.Application.Abstracts.Authentication;
using CustomerManager.Api.Features.Users.Entities;

namespace CustomerManager.Api.Infrastructure.Authorization.Providers
{
    internal sealed class ClaimsProvider : IClaimsProvider
    {
        private readonly IPermissionService _permissionService;
        public ClaimsProvider(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        public async Task<IEnumerable<Claim>> GetClaimsForUser(User user)
        {
            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
            ];

            HashSet<string> permissions = await _permissionService.GetPermissionsAsync(user.Id);

            foreach (string permission in permissions)
            {
                claims.Add(new(CustomJwtClaimTypes.Permissions, permission));
            }

            return claims;
        }

        //private static IEnumerable<Permission> GetPermissionsForUser(User user)
        //{
        //    var permissions = new HashSet<Permission>();

        //    foreach (string roleName in user.Roles)
        //    {
        //        Role role = Role.FromName(roleName).Value;

        //        foreach (Permission permission in role.GetPermissions())
        //        {
        //            permissions.Add(permission);
        //        }
        //    }

        //    return permissions;
        //}
    }
}
