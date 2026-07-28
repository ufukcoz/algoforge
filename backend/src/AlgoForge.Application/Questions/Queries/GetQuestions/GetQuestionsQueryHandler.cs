using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Questions.Queries.GetQuestions;

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, PagedQuestionsDto>
{
    private readonly IApplicationDbContext _context;

    public GetQuestionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedQuestionsDto> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query = _context.Questions.AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(q => q.CategoryId == request.CategoryId.Value);

        if (request.Difficulty.HasValue)
            query = query.Where(q => q.Difficulty == request.Difficulty.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(q => q.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new QuestionSummaryDto(
                q.Id,
                q.Title,
                q.Difficulty.ToString(),
                q.Category.Name
            ))
            .ToListAsync(cancellationToken);

        return new PagedQuestionsDto(items, totalCount, page, pageSize);
    }
}
