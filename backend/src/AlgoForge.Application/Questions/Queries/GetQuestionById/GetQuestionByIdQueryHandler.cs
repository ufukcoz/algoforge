using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Questions.Queries.GetQuestionById;

public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, QuestionDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetQuestionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QuestionDetailDto?> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Category)
            .Include(q => q.TestCases)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

        if (question is null)
            return null;

        // Hidden test case'ler judge tarafından kullanılır (Sprint 4), kullanıcıya asla dönmez.
        var exampleTestCases = question.TestCases
            .Where(tc => !tc.IsHidden)
            .Select(tc => new VisibleTestCaseDto(tc.Input, tc.ExpectedOutput))
            .ToList();

        return new QuestionDetailDto(
            question.Id,
            question.Title,
            question.Difficulty.ToString(),
            question.Description,
            question.TimeLimitMs,
            question.MemoryLimitMb,
            question.Category.Name,
            exampleTestCases
        );
    }
}
