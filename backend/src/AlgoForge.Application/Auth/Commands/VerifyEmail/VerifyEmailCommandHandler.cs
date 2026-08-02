using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public VerifyEmailCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var token = await _context.EmailVerificationTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken);

        if (token is null || !token.IsValid(DateTime.UtcNow))
            return false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == token.UserId, cancellationToken);
        if (user is null)
            return false;

        user.MarkEmailVerified();
        token.MarkUsed();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
