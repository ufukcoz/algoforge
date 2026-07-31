using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
        };
    }

    public bool IsActive(DateTime now) => RevokedAt is null && now < ExpiresAt;

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}
