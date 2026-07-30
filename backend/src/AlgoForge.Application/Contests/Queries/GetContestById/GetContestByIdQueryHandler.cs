using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Contests.Queries.GetContestById;

public class GetContestByIdQueryHandler : IRequestHandler<GetContestByIdQuery, ContestDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetContestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContestDetailDto?> Handle(GetContestByIdQuery request, CancellationToken cancellationToken)
    {
        var contest = await _context.Contests
            .Include(c => c.ContestQuestions)
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == request.ContestId, cancellationToken);

        if (contest is null)
            return null;

        var isJoined = contest.Participants.Any(p => p.UserId == request.CurrentUserId);
        var isCreator = contest.CreatedByUserId == request.CurrentUserId;

        // Private yarismaya katilmamis (ve olusturmamis) kullanici detaylari goremez -
        // sadece davet koduyla katilma denemesi yapabilir.
        if (!contest.IsPublic && !isJoined && !isCreator)
        {
            throw new UnauthorizedAccessException("Bu yarismayi goruntulemek icin davet kodu ile katilman gerekiyor.");
        }

        var questionIds = contest.ContestQuestions.Select(cq => cq.QuestionId).ToList();
        var questions = await _context.Questions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, cancellationToken);

        var now = DateTime.UtcNow;
        var status = now < contest.StartTime ? "Upcoming" : now > contest.EndTime ? "Ended" : "Active";

        var questionDtos = contest.ContestQuestions
            .OrderBy(cq => cq.OrderIndex)
            .Where(cq => questions.ContainsKey(cq.QuestionId))
            .Select(cq => new ContestQuestionDto(
                cq.QuestionId,
                questions[cq.QuestionId].Title,
                questions[cq.QuestionId].Difficulty.ToString(),
                cq.Points,
                cq.OrderIndex
            ))
            .ToList();

        return new ContestDetailDto(
            contest.Id,
            contest.Title,
            contest.Description,
            contest.StartTime,
            contest.EndTime,
            contest.IsPublic,
            isCreator ? contest.InviteCode : null,
            status,
            isJoined,
            contest.Participants.Count,
            questionDtos
        );
    }
}
