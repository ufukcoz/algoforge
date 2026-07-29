using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Questions.Commands.RunCode;

public class RunCodeCommandHandler : IRequestHandler<RunCodeCommand, RunCodeResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJudgeService _judgeService;

    public RunCodeCommandHandler(IApplicationDbContext context, IJudgeService judgeService)
    {
        _context = context;
        _judgeService = judgeService;
    }

    public async Task<RunCodeResult> Handle(RunCodeCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.TestCases)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
            throw new InvalidOperationException("Soru bulunamadi.");

        // "Calistir" butonu sadece ornek (hidden olmayan) test case'lere karsi calisir,
        // boylece kullanici hidden test case'leri gormeden hizli geri bildirim alir.
        var visibleTestCases = question.TestCases.Where(tc => !tc.IsHidden).ToList();

        var results = new List<TestCaseRunResult>();

        foreach (var testCase in visibleTestCases)
        {
            var execution = await _judgeService.ExecuteAsync(
                request.SourceCode,
                request.Language,
                testCase.Input,
                testCase.ExpectedOutput,
                question.TimeLimitMs,
                question.MemoryLimitMb,
                cancellationToken);

            results.Add(new TestCaseRunResult(
                testCase.Input,
                testCase.ExpectedOutput,
                execution.ActualOutput,
                execution.Passed,
                execution.Stderr,
                execution.CompileOutput,
                execution.RuntimeMs
            ));
        }

        return new RunCodeResult(results.All(r => r.Passed) && results.Count > 0, results);
    }
}
