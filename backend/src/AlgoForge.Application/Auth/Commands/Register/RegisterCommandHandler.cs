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

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            throw new InvalidOperationException("Bu e-posta adresi zaten kayıtlı.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Username, request.Email, passwordHash);

        _context.Users.Add(user);

        // Dogrulama tokeni 64 rastgele byte'tan uretiliyor - tahmin edilemez, 24 saat gecerli.
        var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
            .Replace('+', '-').Replace('/', '_').TrimEnd('='); // URL'de sorun cikarmasin diye base64url'e cevir

        var verificationToken = EmailVerificationToken.Create(user.Id, tokenValue, DateTime.UtcNow.AddHours(24));
        _context.EmailVerificationTokens.Add(verificationToken);

        await _context.SaveChangesAsync(cancellationToken);

        // Email gonderimi basarisiz olsa bile kaydin kendisi bozulmasin - kullanici
        // hesabini olusturabilsin, istersen sonradan "dogrulama emailini tekrar gonder" ile dener.
        try
        {
            var baseUrl = _configuration["AppBaseUrl"] ?? "http://localhost:5000";
            var verificationLink = $"{baseUrl}/api/auth/verify-email?token={tokenValue}";

            var htmlBody = $"""
                <h2>AlgoForge'a hos geldin, {user.Username}!</h2>
                <p>Hesabini dogrulamak icin asagidaki linke tikla:</p>
                <p><a href="{verificationLink}">E-postami dogrula</a></p>
                <p>Bu link 24 saat gecerlidir.</p>
                """;

            await _emailService.SendEmailAsync(user.Email, "AlgoForge - E-postani dogrula", htmlBody, cancellationToken);
        }
        catch
        {
            // Sessizce yut - kayit islemi email gonderiminin basarisina bagli olmamali.
            // TODO: Bunu bir logger'a yazip izlemek ileride eklenebilir.
        }

        return new RegisterResult(user.Id, user.Username, user.Email);
    }
}
