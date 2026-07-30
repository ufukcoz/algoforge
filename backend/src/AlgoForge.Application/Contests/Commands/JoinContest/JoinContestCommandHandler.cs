using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Contests.Commands.JoinContest;

public class JoinContestCommandHandler : IRequestHandler<JoinContestCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public JoinContestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(JoinContestCommand request, CancellationToken cancellationToken)
    {
        var contest = await _context.Contests
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == request.ContestId, cancellationToken);

        if (contest is null)
            throw new InvalidOperationException("Yarisma bulunamadi.");

        if (!contest.IsPublic)
        {
            if (string.IsNullOrWhiteSpace(request.InviteCode) ||
                !string.Equals(request.InviteCode, contest.InviteCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Davet kodu hatali veya eksik.");
            }
        }

        contest.AddParticipant(request.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
