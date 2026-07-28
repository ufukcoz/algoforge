using AlgoForge.Domain.Enums;
using MediatR;

namespace AlgoForge.Application.Questions.Queries.GetQuestions;

public record GetQuestionsQuery(
    Guid? CategoryId = null,
    Difficulty? Difficulty = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedQuestionsDto>;

public record QuestionSummaryDto(
    Guid Id,
    string Title,
    string Difficulty,
    string CategoryName
);

public record PagedQuestionsDto(
    List<QuestionSummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
