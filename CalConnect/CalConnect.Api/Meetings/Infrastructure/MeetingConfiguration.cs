using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalConnect.Api.Meetings.Infrastructure;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasMany(p => p.Participants).WithMany();
    }
}