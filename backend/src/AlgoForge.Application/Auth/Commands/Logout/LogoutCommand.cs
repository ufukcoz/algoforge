using MediatR;

namespace AlgoForge.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>;
