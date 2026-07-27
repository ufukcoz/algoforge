using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterResult(user.Id, user.Username, user.Email);
    }
}
