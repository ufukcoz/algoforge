using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == AlgoForge.Domain.Entities.RefreshToken.HashToken(request.RefreshToken), cancellationToken);

        // Token zaten yoksa veya iptal edilmisse sessizce basarili donuyoruz -
        // logout islemi idempotent olmali, hata firlatmaya gerek yok.
        token?.Revoke();

        if (token is not null)
            await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

