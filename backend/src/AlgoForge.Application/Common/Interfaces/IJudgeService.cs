namespace AlgoForge.Application.Common.Interfaces;

public record JudgeExecutionResult(
    bool Passed,
    string? ActualOutput,
    string? Stderr,
    string? CompileOutput,
    int? RuntimeMs,
    int? MemoryKb,
    string StatusDescription
);

// Bu arayuz sayesinde Judge0'i RapidAPI'den kendi Docker instance'imiza
// gecirmek istersek sadece Infrastructure'daki implementasyonu degistirmemiz yeterli;
// Application katmani Judge0'a ozgu hicbir seyi bilmez.
public interface IJudgeService
{
    Task<JudgeExecutionResult> ExecuteAsync(
        string sourceCode,
        string language,
        string stdin,
        string expectedOutput,
        int timeLimitMs,
        int memoryLimitMb,
        CancellationToken cancellationToken);
}
