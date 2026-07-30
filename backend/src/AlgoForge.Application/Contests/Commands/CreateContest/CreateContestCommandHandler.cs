using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Security.Cryptography;

namespace AlgoForge.Application.Contests.Commands.CreateContest;

public class CreateContestCommandHandler : IRequestHandler<CreateContestCommand, Guid>
{
    private const string InviteCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // karisikligi onlemek icin 0/O, 1/I cikarildi

    private readonly IApplicationDbContext _context;

    public CreateContestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateContestCommand request, CancellationToken cancellationToken)
    {
        if (request.Questions.Count == 0)
            throw new InvalidOperationException("Yarismaya en az bir soru eklenmeli.");

        var questionIds = request.Questions.Select(q => q.QuestionId).ToList();
        var existingCount = await _context.Questions.CountAsync(q => questionIds.Contains(q.Id), cancellationToken);
        if (existingCount != questionIds.Distinct().Count())
            throw new InvalidOperationException("Belirtilen sorulardan biri veya birkaci bulunamadi.");

        var inviteCode = request.IsPublic ? null : GenerateInviteCode();

        var contest = Contest.Create(
            request.Title,
            request.Description,
            request.StartTime,
            request.EndTime,
            request.CreatedByUserId,
            request.IsPublic,
            inviteCode
        );

        var orderIndex = 0;
        foreach (var q in request.Questions)
        {
            contest.AddQuestion(q.QuestionId, q.Points, orderIndex++);
        }

        // Yaratici otomatik olarak ilk katilimci olur.
        contest.AddParticipant(request.CreatedByUserId);

        _context.Contests.Add(contest);
        await _context.SaveChangesAsync(cancellationToken);

        return contest.Id;
    }

    private static string GenerateInviteCode()
    {
        var bytes = RandomNumberGenerator.GetBytes(8);
        var chars = new char[8];
        for (var i = 0; i < 8; i++)
        {
            chars[i] = InviteCodeAlphabet[bytes[i] % InviteCodeAlphabet.Length];
        }
        return new string(chars);
    }
}
