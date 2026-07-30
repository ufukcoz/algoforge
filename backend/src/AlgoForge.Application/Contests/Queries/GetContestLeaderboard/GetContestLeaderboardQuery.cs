using MediatR;

namespace AlgoForge.Application.Contests.Queries.GetContestLeaderboard;

public record GetContestLeaderboardQuery(Guid ContestId) : IRequest<List<ContestLeaderboardEntryDto>>;

public record ContestLeaderboardEntryDto(
    int Rank,
    string Username,
    int TotalPoints,
    int SolvedCount,
    // Ilk cozumden yarismanin son cozumune kadar gecen sure (saniye) - esitlik durumunda
    // daha erken bitiren one gecer (klasik ICPC-tarzi tiebreak).
    int TotalPenaltySeconds
);
