using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.ToTable("EmailVerificationTokens");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token).IsRequired().HasMaxLength(200);
        builder.Property(t => t.ExpiresAt).IsRequired();

        builder.HasIndex(t => t.Token).IsUnique();
        builder.HasIndex(t => t.UserId);
    }
}
