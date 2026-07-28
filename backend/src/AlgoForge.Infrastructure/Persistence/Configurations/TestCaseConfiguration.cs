using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.ToTable("TestCases");
        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Input).IsRequired();
        builder.Property(tc => tc.ExpectedOutput).IsRequired();
        builder.Property(tc => tc.IsHidden).IsRequired();

        builder.HasIndex(tc => tc.QuestionId);
    }
}
