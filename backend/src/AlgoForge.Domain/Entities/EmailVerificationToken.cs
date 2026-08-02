using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class EmailVerificationToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    private EmailVerificationToken() { }

    public static EmailVerificationToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new EmailVerificationToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
        };
    }

    public bool IsValid(DateTime now) => UsedAt is null && now < ExpiresAt;

    public void MarkUsed()
    {
        UsedAt = DateTime.UtcNow;
    }
}
