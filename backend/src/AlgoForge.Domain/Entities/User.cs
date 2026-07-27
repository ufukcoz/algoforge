using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public int Xp { get; private set; }
    public int Level { get; private set; } = 1;
    public string? Country { get; private set; }
    public string? University { get; private set; }
    public bool EmailVerified { get; private set; }

    private User() { }

    public static User Create(string username, string email, string passwordHash)
    {
        return new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };
    }

    public void AddXp(int amount)
    {
        if (amount <= 0) return;
        Xp += amount;
        Level = 1 + Xp / 1000; // basit seviye kuralı, ileride ayarlanabilir
    }

    public void MarkEmailVerified() => EmailVerified = true;
}
