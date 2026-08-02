using MediatR;

namespace AlgoForge.Application.Auth.Commands.ResendVerificationEmail;

public record ResendVerificationEmailCommand(Guid UserId) : IRequest<Unit>;
