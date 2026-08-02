using MediatR;

namespace AlgoForge.Application.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<bool>;
