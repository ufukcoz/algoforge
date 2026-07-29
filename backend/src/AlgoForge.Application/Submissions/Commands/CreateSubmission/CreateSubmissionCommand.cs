using MediatR;

namespace AlgoForge.Application.Submissions.Commands.CreateSubmission;

public record CreateSubmissionCommand(
    Guid UserId,
    Guid QuestionId,
    string Language,
    string SourceCode
) : IRequest<SubmissionResultDto>;

public record SubmissionResultDto(
    Guid SubmissionId,
    string Status,
    int PassedCount,
    int TotalCount,
    int? RuntimeMs,
    int? MemoryKb
);
