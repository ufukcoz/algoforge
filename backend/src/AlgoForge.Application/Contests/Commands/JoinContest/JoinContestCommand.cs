using MediatR;

namespace AlgoForge.Application.Contests.Commands.JoinContest;

// InviteCode sadece private yarismalar icin gerekli; public yarismada null gecilebilir.
public record JoinContestCommand(Guid UserId, Guid ContestId, string? InviteCode) : IRequest<Unit>;
