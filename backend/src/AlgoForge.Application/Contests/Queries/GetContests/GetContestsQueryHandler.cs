using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Contests.Queries.GetContests;

public class GetContestsQueryHandler : IRequestHandler<GetContestsQuery, List<ContestSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetContestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ContestSummaryDto>> Handle(GetContestsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Kullaniciya gorunur yarismalar: tum public yarismalar + zaten katildigi private yarismalar.
        var visibleContests = await _context.Contests
            .Where(c => c.IsPublic || c.Participants.Any(p => p.UserId == request.CurrentUserId))
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.StartTime,
                c.EndTime,
                c.IsPublic,
                ParticipantCount = c.Participants.Count,
                QuestionCount = c.ContestQuestions.Count,
                IsJoined = c.Participants.Any(p => p.UserId == request.CurrentUserId),
            })
            .OrderByDescending(c => c.StartTime)
            .ToListAsync(cancellationToken);

        return visibleContests.Select(c => new ContestSummaryDto(
            c.Id,
            c.Title,
            c.StartTime,
            c.EndTime,
            c.IsPublic,
            c.ParticipantCount,
            c.QuestionCount,
            GetStatus(c.StartTime, c.EndTime, now),
            c.IsJoined
        )).ToList();
    }

    private static string GetStatus(DateTime start, DateTime end, DateTime now)
    {
        if (now < start) return "Upcoming";
        if (now > end) return "Ended";
        return "Active";
    }
}
