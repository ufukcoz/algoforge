using AlgoForge.Domain.Common;
using AlgoForge.Domain.Enums;

namespace AlgoForge.Domain.Entities;

public class Submission : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Language { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public SubmissionStatus Status { get; private set; }
    public int? RuntimeMs { get; private set; }
    public int? MemoryKb { get; private set; }

    private Submission() { }

    public static Submission Create(Guid userId, Guid questionId, string language, string code)
    {
        return new Submission
        {
            UserId = userId,
            QuestionId = questionId,
            Language = language,
            Code = code,
            Status = SubmissionStatus.Pending,
        };
    }

    public void MarkResult(SubmissionStatus status, int? runtimeMs, int? memoryKb)
    {
        Status = status;
        RuntimeMs = runtimeMs;
        MemoryKb = memoryKb;
    }
}
