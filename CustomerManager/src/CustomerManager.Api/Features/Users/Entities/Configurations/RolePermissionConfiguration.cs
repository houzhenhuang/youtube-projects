using CustomerManager.Api.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EPermission = CustomerManager.Api.Infrastructure.Authorization.Enums.Permission;

namespace CustomerManager.Api.Features.Users.Entities.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.HasData(Create(Role.Registered, EPermission.UserRead, EPermission.UserModify));
    }

    private static RolePermission[] Create(Role role, params EPermission[] permissions)
        => permissions.Select(permission => new RolePermission
        {
            RoleId = role.Id,
            PermissionId = (int)permission
        }).ToArray();
}
