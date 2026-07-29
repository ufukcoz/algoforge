using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, SubmissionResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJudgeService _judgeService;

    public CreateSubmissionCommandHandler(IApplicationDbContext context, IJudgeService judgeService)
    {
        _context = context;
        _judgeService = judgeService;
    }

    public async Task<SubmissionResultDto> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.TestCases)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
            throw new InvalidOperationException("Soru bulunamadi.");

        var submission = Submission.Create(request.UserId, request.QuestionId, request.Language, request.SourceCode);
        _context.Submissions.Add(submission);

        // Submit, hidden test case'ler dahil TUM test case'lere karsi calisir.
        // Bu, Run Code'dan (sadece ornekler) en onemli fark.
        var allTestCases = question.TestCases.ToList();
        var passedCount = 0;
        var overallStatus = SubmissionStatus.Accepted;
        int? maxRuntimeMs = null;
        int? maxMemoryKb = null;

        foreach (var testCase in allTestCases)
        {
            var execution = await _judgeService.ExecuteAsync(
                request.SourceCode,
                request.Language,
                testCase.Input,
                testCase.ExpectedOutput,
                question.TimeLimitMs,
                question.MemoryLimitMb,
                cancellationToken);

            if (execution.RuntimeMs.HasValue)
                maxRuntimeMs = Math.Max(maxRuntimeMs ?? 0, execution.RuntimeMs.Value);
            if (execution.MemoryKb.HasValue)
                maxMemoryKb = Math.Max(maxMemoryKb ?? 0, execution.MemoryKb.Value);

            if (execution.Passed)
            {
                passedCount++;
                continue;
            }

            // Ilk basarisiz test case'in durumunu genel sonuc olarak isaretle,
            // ama kalan test case'leri de calistirmaya devam et (toplam pass sayisini gormek icin).
            if (overallStatus == SubmissionStatus.Accepted)
            {
                overallStatus = MapToStatus(execution.StatusDescription);
            }
        }

        if (allTestCases.Count == 0)
            overallStatus = SubmissionStatus.InternalError;

        submission.MarkResult(overallStatus, maxRuntimeMs, maxMemoryKb);
        await _context.SaveChangesAsync(cancellationToken);

        return new SubmissionResultDto(
            submission.Id,
            overallStatus.ToString(),
            passedCount,
            allTestCases.Count,
            maxRuntimeMs,
            maxMemoryKb
        );
    }

    private static SubmissionStatus MapToStatus(string judge0StatusDescription)
    {
        // Judge0'in dondurdugu status.description degerleri (RapidAPI CE):
        // "Accepted", "Wrong Answer", "Time Limit Exceeded", "Compilation Error",
        // "Runtime Error (SIGSEGV)", "Runtime Error (NZEC)", vb.
        var normalized = judge0StatusDescription.ToLowerInvariant();

        if (normalized.Contains("time limit"))
            return SubmissionStatus.TimeLimitExceeded;
        if (normalized.Contains("compilation"))
            return SubmissionStatus.CompileError;
        if (normalized.Contains("runtime error"))
            return SubmissionStatus.RuntimeError;
        if (normalized.Contains("wrong answer"))
            return SubmissionStatus.WrongAnswer;
        if (normalized.Contains("accepted"))
            return SubmissionStatus.Accepted;

        return SubmissionStatus.InternalError;
    }
}
