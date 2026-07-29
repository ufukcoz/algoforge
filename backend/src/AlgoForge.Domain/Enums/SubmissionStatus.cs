namespace AlgoForge.Domain.Enums;

public enum SubmissionStatus
{
    Pending = 0,
    Accepted = 1,
    WrongAnswer = 2,
    TimeLimitExceeded = 3,
    RuntimeError = 4,
    CompileError = 5,
    InternalError = 6,
}
