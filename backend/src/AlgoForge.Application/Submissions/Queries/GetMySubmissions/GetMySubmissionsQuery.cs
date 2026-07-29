using MediatR;

namespace AlgoForge.Application.Submissions.Queries.GetMySubmissions;

public record GetMySubmissionsQuery(Guid UserId, Guid? QuestionId = null) : IRequest<List<SubmissionSummaryDto>>;

public record SubmissionSummaryDto(
    Guid Id,
    string QuestionTitle,
    string Language,
    string Status,
    int? RuntimeMs,
    int? MemoryKb,
    DateTime CreatedAt
);
