using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title).IsRequired().HasMaxLength(200);
        builder.Property(q => q.Description).IsRequired();
        builder.Property(q => q.Difficulty).HasConversion<int>().IsRequired();
        builder.Property(q => q.TimeLimitMs).IsRequired();
        builder.Property(q => q.MemoryLimitMb).IsRequired();

        builder.HasOne(q => q.Category)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.TestCases)
            .WithOne()
            .HasForeignKey(tc => tc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.CategoryId);
        builder.HasIndex(q => q.Difficulty);
    }
}
