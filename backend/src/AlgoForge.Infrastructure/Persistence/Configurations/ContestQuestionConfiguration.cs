using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class ContestQuestionConfiguration : IEntityTypeConfiguration<ContestQuestion>
{
    public void Configure(EntityTypeBuilder<ContestQuestion> builder)
    {
        builder.ToTable("ContestQuestions");
        builder.HasKey(cq => cq.Id);

        builder.Property(cq => cq.Points).IsRequired();
        builder.Property(cq => cq.OrderIndex).IsRequired();

        builder.HasIndex(cq => cq.ContestId);
        builder.HasIndex(cq => cq.QuestionId);
    }
}
