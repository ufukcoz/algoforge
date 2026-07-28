using AlgoForge.Domain.Enums;
using MediatR;

namespace AlgoForge.Application.Questions.Commands.CreateQuestion;

public record CreateQuestionCommand(
    string Title,
    Difficulty Difficulty,
    string Description,
    Guid CategoryId,
    int TimeLimitMs,
    int MemoryLimitMb,
    List<CreateTestCaseDto> TestCases
) : IRequest<Guid>;

public record CreateTestCaseDto(string Input, string ExpectedOutput, bool IsHidden);
