using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Leaderboard.Queries.GetLeaderboard;

public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, List<LeaderboardEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLeaderboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaderboardEntryDto>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var top = request.Top is < 1 or > 200 ? 50 : request.Top;

        var users = await _context.Users
            .OrderByDescending(u => u.Xp)
            .Take(top)
            .Select(u => new { u.Username, u.Xp, u.Level })
            .ToListAsync(cancellationToken);

        return users
            .Select((u, index) => new LeaderboardEntryDto(index + 1, u.Username, u.Xp, u.Level))
            .ToList();
    }
}
