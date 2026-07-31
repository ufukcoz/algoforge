using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        // Refresh token'i veritabanina kaydediyoruz ki /api/auth/refresh cagrildiginda
        // gercekten bu kullaniciya ait, iptal edilmemis, suresi gecmemis bir token mi diye
        // kontrol edebilelim. Onceden bu deger sadece uretilip hic saklanmiyordu.
        var refreshToken = AlgoForge.Domain.Entities.RefreshToken.Create(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(30));
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshTokenValue, user.Username);
    }
}
