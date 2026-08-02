using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace AlgoForge.Application.Auth.Commands.ResendVerificationEmail;

public class ResendVerificationEmailCommandHandler : IRequestHandler<ResendVerificationEmailCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public ResendVerificationEmailCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null || user.EmailVerified)
            return Unit.Value; // zaten dogrulanmis ya da kullanici yok - sessizce cik

        var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');

        var verificationToken = EmailVerificationToken.Create(user.Id, tokenValue, DateTime.UtcNow.AddHours(24));
        _context.EmailVerificationTokens.Add(verificationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var baseUrl = _configuration["AppBaseUrl"] ?? "http://localhost:5000";
        var verificationLink = $"{baseUrl}/api/auth/verify-email?token={tokenValue}";

        var htmlBody = $"""
            <h2>AlgoForge - E-posta dogrulama</h2>
            <p>Hesabini dogrulamak icin asagidaki linke tikla:</p>
            <p><a href="{verificationLink}">E-postami dogrula</a></p>
            <p>Bu link 24 saat gecerlidir.</p>
            """;

        await _emailService.SendEmailAsync(user.Email, "AlgoForge - E-postani dogrula", htmlBody, cancellationToken);

        return Unit.Value;
    }
}
