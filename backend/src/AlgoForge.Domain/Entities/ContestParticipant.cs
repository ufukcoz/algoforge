using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class ContestParticipant : BaseEntity
{
    public Guid ContestId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private ContestParticipant() { }

    public static ContestParticipant Create(Guid contestId, Guid userId)
    {
        return new ContestParticipant
        {
            ContestId = contestId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
        };
    }
}
