using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users.Infrastructure;

internal sealed class EmailVerificationTokenConfiguration: IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
    }
}