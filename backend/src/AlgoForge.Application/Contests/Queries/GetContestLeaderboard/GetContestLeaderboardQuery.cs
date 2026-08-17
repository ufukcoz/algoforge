using MediatR;

namespace AlgoForge.Application.Contests.Queries.GetContestLeaderboard;

public record GetContestLeaderboardQuery(
    Guid ContestId,
    Guid CurrentUserId
) : IRequest<List<ContestLeaderboardEntryDto>>;

public record ContestLeaderboardEntryDto(
    int Rank,
    string Username,
    int TotalPoints,
    int SolvedCount,
    int TotalPenaltySeconds
);
