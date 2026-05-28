using CustomerManager.Api.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenApiSamples.Constants;

namespace CustomerManager.Api.Features.Users.Entities.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.PasswordHash).HasMaxLength(32);

        builder.HasMany(x => x.Roles)
            .WithMany()
            .UsingEntity<UserRole>();

        builder.HasData(User.GetValues());
    }
}
