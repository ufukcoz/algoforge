using MediatR;

namespace AlgoForge.Application.Questions.Queries.GetQuestionById;

public record GetQuestionByIdQuery(Guid Id) : IRequest<QuestionDetailDto?>;

public record VisibleTestCaseDto(string Input, string ExpectedOutput);

public record QuestionDetailDto(
    Guid Id,
    string Title,
    string Difficulty,
    string Description,
    int TimeLimitMs,
    int MemoryLimitMb,
    string CategoryName,
    List<VisibleTestCaseDto> ExampleTestCases
);
