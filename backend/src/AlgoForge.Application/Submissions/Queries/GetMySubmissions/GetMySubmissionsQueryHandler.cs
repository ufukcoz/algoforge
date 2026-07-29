using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Submissions.Queries.GetMySubmissions;

public class GetMySubmissionsQueryHandler : IRequestHandler<GetMySubmissionsQuery, List<SubmissionSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMySubmissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubmissionSummaryDto>> Handle(GetMySubmissionsQuery request, CancellationToken cancellationToken)
    {
        var query =
            from submission in _context.Submissions
            where submission.UserId == request.UserId
            join question in _context.Questions on submission.QuestionId equals question.Id
            select new { submission, question };

        if (request.QuestionId.HasValue)
            query = query.Where(x => x.submission.QuestionId == request.QuestionId.Value);

        var result = await query
            .OrderByDescending(x => x.submission.CreatedAt)
            .Select(x => new SubmissionSummaryDto(
                x.submission.Id,
                x.question.Title,
                x.submission.Language,
                x.submission.Status.ToString(),
                x.submission.RuntimeMs,
                x.submission.MemoryKb,
                x.submission.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return result;
    }
}
