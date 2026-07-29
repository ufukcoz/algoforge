using MediatR;

namespace AlgoForge.Application.Leaderboard.Queries.GetLeaderboard;

public record GetLeaderboardQuery(int Top = 50) : IRequest<List<LeaderboardEntryDto>>;

public record LeaderboardEntryDto(
    int Rank,
    string Username,
    int Xp,
    int Level
);
