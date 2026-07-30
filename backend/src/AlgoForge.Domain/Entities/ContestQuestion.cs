using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class ContestQuestion : BaseEntity
{
    public Guid ContestId { get; private set; }
    public Guid QuestionId { get; private set; }
    public int Points { get; private set; }
    public int OrderIndex { get; private set; }

    private ContestQuestion() { }

    public static ContestQuestion Create(Guid contestId, Guid questionId, int points, int orderIndex)
    {
        return new ContestQuestion
        {
            ContestId = contestId,
            QuestionId = questionId,
            Points = points,
            OrderIndex = orderIndex,
        };
    }
}
