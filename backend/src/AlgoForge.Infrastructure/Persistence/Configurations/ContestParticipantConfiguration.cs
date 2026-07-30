using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class ContestParticipantConfiguration : IEntityTypeConfiguration<ContestParticipant>
{
    public void Configure(EntityTypeBuilder<ContestParticipant> builder)
    {
        builder.ToTable("ContestParticipants");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.JoinedAt).IsRequired();

        builder.HasIndex(p => new { p.ContestId, p.UserId }).IsUnique();
    }
}
