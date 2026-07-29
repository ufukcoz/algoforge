using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Language).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Code).IsRequired();
        builder.Property(s => s.Status).HasConversion<int>().IsRequired();

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.QuestionId);
    }
}
