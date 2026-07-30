using MediatR;

namespace AlgoForge.Application.Contests.Queries.GetContests;

public record GetContestsQuery(Guid CurrentUserId) : IRequest<List<ContestSummaryDto>>;

public record ContestSummaryDto(
    Guid Id,
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    bool IsPublic,
    int ParticipantCount,
    int QuestionCount,
    string Status, // "Upcoming" | "Active" | "Ended"
    bool IsJoined
);
