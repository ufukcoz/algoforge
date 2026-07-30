using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Contests.Queries.GetContestLeaderboard;

public class GetContestLeaderboardQueryHandler : IRequestHandler<GetContestLeaderboardQuery, List<ContestLeaderboardEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetContestLeaderboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ContestLeaderboardEntryDto>> Handle(GetContestLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var contest = await _context.Contests
            .Include(c => c.ContestQuestions)
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == request.ContestId, cancellationToken);

        if (contest is null)
            throw new InvalidOperationException("Yarisma bulunamadi.");

        var pointsByQuestion = contest.ContestQuestions.ToDictionary(cq => cq.QuestionId, cq => cq.Points);
        var questionIds = pointsByQuestion.Keys.ToList();
        var participantUserIds = contest.Participants.Select(p => p.UserId).ToList();

        // Yarisma penceresi icindeki, ilgili sorulara ait, ilgili katilimcilarin TUM submission'lari.
        // Puanlama icin sadece Accepted olanlar kullanilir; digerleri yok sayilir.
        var relevantSubmissions = await _context.Submissions
            .Where(s => participantUserIds.Contains(s.UserId)
                        && questionIds.Contains(s.QuestionId)
                        && s.CreatedAt >= contest.StartTime
                        && s.CreatedAt <= contest.EndTime
                        && s.Status == SubmissionStatus.Accepted)
            .Select(s => new { s.UserId, s.QuestionId, s.CreatedAt })
            .ToListAsync(cancellationToken);

        var usernameById = await _context.Users
            .Where(u => participantUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Username, cancellationToken);

        var entries = new List<(Guid UserId, int Points, int Solved, double PenaltySeconds)>();

        foreach (var userId in participantUserIds)
        {
            // Her soru icin bu kullanicinin ILK basarili cozumunu buluyoruz (en erken CreatedAt).
            var firstAcceptedPerQuestion = relevantSubmissions
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.QuestionId)
                .Select(g => new { QuestionId = g.Key, FirstAcceptedAt = g.Min(x => x.CreatedAt) })
                .ToList();

            var totalPoints = 0;
            var totalPenaltySeconds = 0.0;

            foreach (var solved in firstAcceptedPerQuestion)
            {
                totalPoints += pointsByQuestion.GetValueOrDefault(solved.QuestionId, 0);
                totalPenaltySeconds += (solved.FirstAcceptedAt - contest.StartTime).TotalSeconds;
            }

            entries.Add((userId, totalPoints, firstAcceptedPerQuestion.Count, totalPenaltySeconds));
        }

        var ranked = entries
            .OrderByDescending(e => e.Points)
            .ThenBy(e => e.PenaltySeconds)
            .Select((e, index) => new ContestLeaderboardEntryDto(
                index + 1,
                usernameById.GetValueOrDefault(e.UserId, "bilinmiyor"),
                e.Points,
                e.Solved,
                (int)e.PenaltySeconds
            ))
            .ToList();

        return ranked;
    }
}
