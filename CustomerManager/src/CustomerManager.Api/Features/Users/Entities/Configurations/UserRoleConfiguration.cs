using CustomerManager.Api.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerManager.Api.Features.Users.Entities.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasData(Create(User.DefaultUser, Role.Registered));
    }

    private static UserRole Create(User user, Role role)
    {
        return new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
        };

    }
}
