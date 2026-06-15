using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalConnect.Api.Meetings.Infrastructure;

public class AgendaItemConfiguration : IEntityTypeConfiguration<AgendaItem>
{
    public void Configure(EntityTypeBuilder<AgendaItem> builder)
    {
        builder.HasKey(m => m.Id);
    }
}