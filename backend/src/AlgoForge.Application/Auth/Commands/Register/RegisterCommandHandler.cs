using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace AlgoForge.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<RegisterResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            throw new InvalidOperationException(
                "Bu e-posta adresi zaten kayıtlı.");

        // Password strength validation
        if (request.Password.Length < 8 ||
            !request.Password.Any(char.IsUpper) ||
            !request.Password.Any(char.IsLower) ||
            !request.Password.Any(char.IsDigit))
        {
            throw new ArgumentException(
                "Şifre en az 8 karakter, bir büyük harf, bir küçük harf ve bir rakam içermelidir.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        // New users are always created with the default User role.
        var user = User.Create(
            request.Username,
            request.Email,
            passwordHash);

        _context.Users.Add(user);

        // Verification token is generated from 48 random bytes.
        // The token is unpredictable and valid for 24 hours.
        var tokenValue = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(48))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        var verificationToken = EmailVerificationToken.Create(
            user.Id,
            tokenValue,
            DateTime.UtcNow.AddHours(24));

        _context.EmailVerificationTokens.Add(verificationToken);

        await _context.SaveChangesAsync(cancellationToken);

        // Email delivery failure should not cancel account creation.
        // The user can request another verification email later.
        try
        {
            var baseUrl = _configuration["AppBaseUrl"]
                          ?? "http://localhost:5000";

            var verificationLink =
                $"{baseUrl}/api/auth/verify-email?token={tokenValue}";

            var htmlBody = $"""
                <h2>AlgoForge'a hoş geldin, {user.Username}!</h2>
                <p>Hesabını doğrulamak için aşağıdaki linke tıkla:</p>
                <p>
                    <a href="{verificationLink}">
                        E-postamı doğrula
                    </a>
                </p>
                <p>Bu link 24 saat geçerlidir.</p>
                """;

            await _emailService.SendEmailAsync(
                user.Email,
                "AlgoForge - E-postanı doğrula",
                htmlBody,
                cancellationToken);
        }
        catch
        {
            // Email delivery failure does not prevent registration.
            // TODO: Add logging here in the future.
        }

        return new RegisterResult(
            user.Id,
            user.Username,
            user.Email);
    }
}