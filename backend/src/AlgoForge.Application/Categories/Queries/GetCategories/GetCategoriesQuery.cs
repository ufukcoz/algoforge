using MediatR;

namespace AlgoForge.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public record CategoryDto(Guid Id, string Name, int QuestionCount);
