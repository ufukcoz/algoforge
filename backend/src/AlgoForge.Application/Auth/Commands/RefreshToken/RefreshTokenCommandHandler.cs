using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RefreshTokenResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            AlgoForge.Domain.Entities.RefreshToken.HashToken(
                request.RefreshToken);

        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                rt => rt.TokenHash == tokenHash,
                cancellationToken);

        if (existingToken is null)
        {
            throw new UnauthorizedAccessException(
                "Refresh token geçersiz veya süresi dolmuş, tekrar giriş yapman gerekiyor.");
        }

        // Daha önce kullanılmış bir token tekrar gönderildiyse
        // refresh-token reuse saldırısı tespit edilmiş olur.
        if (existingToken.RevokedAt is not null)
        {
            var activeFamilyTokens = await _context.RefreshTokens
                .Where(rt =>
                    rt.FamilyId == existingToken.FamilyId &&
                    rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var familyToken in activeFamilyTokens)
            {
                familyToken.Revoke();
            }

            await _context.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedAccessException(
                "Refresh token reuse detected. Oturum güvenlik nedeniyle sonlandırıldı.");
        }

        if (!existingToken.IsActive(DateTime.UtcNow))
        {
            throw new UnauthorizedAccessException(
                "Refresh token süresi dolmuş, tekrar giriş yapman gerekiyor.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == existingToken.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Kullanıcı bulunamadı.");
        }

        // Token rotation.
        // Eski token bu noktadan sonra tekrar kullanılamaz.
        existingToken.Revoke();

        var newAccessToken =
            _jwtService.GenerateAccessToken(user);

        var newRefreshTokenValue =
            _jwtService.GenerateRefreshToken();

        // Aynı refresh-token family devam ediyor.
        var newRefreshToken =
            AlgoForge.Domain.Entities.RefreshToken.Create(
                user.Id,
                newRefreshTokenValue,
                DateTime.UtcNow.AddDays(30),
                existingToken.FamilyId);

        _context.RefreshTokens.Add(newRefreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(
            newAccessToken,
            newRefreshTokenValue);
    }
}
