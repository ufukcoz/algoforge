using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == AlgoForge.Domain.Entities.RefreshToken.HashToken(request.RefreshToken), cancellationToken);

        if (existingToken is null || !existingToken.IsActive(DateTime.UtcNow))
            throw new UnauthorizedAccessException("Refresh token gecersiz veya suresi dolmus, tekrar giris yapman gerekiyor.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingToken.UserId, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("Kullanici bulunamadi.");

        // Token rotation: eski refresh token'i iptal edip yenisini veriyoruz.
        // Boylece calinmis bir refresh token sonsuza kadar kullanilamaz - her kullanimda
        // yenisiyle degisir, eski gecersiz kalir.
        existingToken.Revoke();

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
        var newRefreshToken = AlgoForge.Domain.Entities.RefreshToken.Create(
            user.Id, newRefreshTokenValue, DateTime.UtcNow.AddDays(30));

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(newAccessToken, newRefreshTokenValue);
    }
}

