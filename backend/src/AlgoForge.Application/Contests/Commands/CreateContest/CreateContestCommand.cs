using MediatR;

namespace AlgoForge.Application.Contests.Commands.CreateContest;

public record CreateContestCommand(
    Guid CreatedByUserId,
    string Title,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    bool IsPublic,
    List<ContestQuestionInput> Questions
) : IRequest<Guid>;

public record ContestQuestionInput(Guid QuestionId, int Points);
