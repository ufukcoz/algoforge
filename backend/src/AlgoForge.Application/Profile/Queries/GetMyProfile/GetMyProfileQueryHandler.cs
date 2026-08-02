using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileDto>
{
    private readonly IApplicationDbContext _context;

    public GetMyProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("Kullanici bulunamadi.");

        var submissions = _context.Submissions.Where(s => s.UserId == request.UserId);

        var totalSubmissions = await submissions.CountAsync(cancellationToken);
        var acceptedSubmissions = await submissions.CountAsync(s => s.Status == SubmissionStatus.Accepted, cancellationToken);
        var questionsSolved = await submissions
            .Where(s => s.Status == SubmissionStatus.Accepted)
            .Select(s => s.QuestionId)
            .Distinct()
            .CountAsync(cancellationToken);

        return new ProfileDto(
            user.Username,
            user.Email,
            user.EmailVerified,
            user.Xp,
            user.Level,
            user.Country,
            user.University,
            user.CreatedAt,
            totalSubmissions,
            acceptedSubmissions,
            questionsSolved
        );
    }
}
