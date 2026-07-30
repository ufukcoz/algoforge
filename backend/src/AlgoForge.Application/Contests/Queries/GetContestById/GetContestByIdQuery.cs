using MediatR;

namespace AlgoForge.Application.Contests.Queries.GetContestById;

public record GetContestByIdQuery(Guid ContestId, Guid CurrentUserId) : IRequest<ContestDetailDto?>;

public record ContestQuestionDto(Guid QuestionId, string Title, string Difficulty, int Points, int OrderIndex);

public record ContestDetailDto(
    Guid Id,
    string Title,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    bool IsPublic,
    // InviteCode sadece yarismayi olusturan kisiye donulur, baskasina sizdirilmaz.
    string? InviteCode,
    string Status,
    bool IsJoined,
    int ParticipantCount,
    List<ContestQuestionDto> Questions
);
