using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class ContestConfiguration : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.ToTable("Contests");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).IsRequired();
        builder.Property(c => c.InviteCode).HasMaxLength(16);

        builder.HasMany(c => c.ContestQuestions)
            .WithOne()
            .HasForeignKey(cq => cq.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Participants)
            .WithOne()
            .HasForeignKey(p => p.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.StartTime);
        builder.HasIndex(c => c.InviteCode);
    }
}
