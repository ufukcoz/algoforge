using MediatR;

namespace AlgoForge.Application.Questions.Commands.RunCode;

public record RunCodeCommand(Guid QuestionId, string Language, string SourceCode) : IRequest<RunCodeResult>;

public record TestCaseRunResult(
    string Input,
    string ExpectedOutput,
    string? ActualOutput,
    bool Passed,
    string? Stderr,
    string? CompileOutput,
    int? RuntimeMs
);

public record RunCodeResult(bool AllPassed, List<TestCaseRunResult> Results);
