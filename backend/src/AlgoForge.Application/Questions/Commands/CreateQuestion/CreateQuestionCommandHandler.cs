using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Questions.Commands.CreateQuestion;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateQuestionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new InvalidOperationException("Belirtilen kategori bulunamadi.");

        if (request.TestCases.Count == 0)
            throw new InvalidOperationException("En az bir test case gerekli.");

        var question = Question.Create(
            request.Title,
            request.Difficulty,
            request.Description,
            request.CategoryId,
            request.TimeLimitMs,
            request.MemoryLimitMb
        );

        foreach (var testCase in request.TestCases)
        {
            question.AddTestCase(testCase.Input, testCase.ExpectedOutput, testCase.IsHidden);
        }

        _context.Questions.Add(question);
        await _context.SaveChangesAsync(cancellationToken);

        return question.Id;
    }
}
