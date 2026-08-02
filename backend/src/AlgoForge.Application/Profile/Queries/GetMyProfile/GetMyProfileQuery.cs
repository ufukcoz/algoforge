using MediatR;

namespace AlgoForge.Application.Profile.Queries.GetMyProfile;

public record GetMyProfileQuery(Guid UserId) : IRequest<ProfileDto>;

public record ProfileDto(
    string Username,
    string Email,
    bool EmailVerified,
    int Xp,
    int Level,
    string? Country,
    string? University,
    DateTime MemberSince,
    int TotalSubmissions,
    int AcceptedSubmissions,
    int QuestionsSolved
);
