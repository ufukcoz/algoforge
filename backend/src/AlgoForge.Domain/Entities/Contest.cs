using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class Contest : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public bool IsPublic { get; private set; }
    // Ozel yarismalar icin davet kodu - IsPublic=false oldugunda kullanilir.
    public string? InviteCode { get; private set; }

    private readonly List<ContestQuestion> _contestQuestions = new();
    public IReadOnlyCollection<ContestQuestion> ContestQuestions => _contestQuestions.AsReadOnly();

    private readonly List<ContestParticipant> _participants = new();
    public IReadOnlyCollection<ContestParticipant> Participants => _participants.AsReadOnly();

    private Contest() { }

    public static Contest Create(
        string title,
        string description,
        DateTime startTime,
        DateTime endTime,
        Guid createdByUserId,
        bool isPublic,
        string? inviteCode = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("Bitis zamani baslangic zamanindan sonra olmali.");

        return new Contest
        {
            Title = title,
            Description = description,
            StartTime = startTime,
            EndTime = endTime,
            CreatedByUserId = createdByUserId,
            IsPublic = isPublic,
            InviteCode = isPublic ? null : inviteCode,
        };
    }

    public void AddQuestion(Guid questionId, int points, int orderIndex)
    {
        _contestQuestions.Add(ContestQuestion.Create(Id, questionId, points, orderIndex));
    }

    public void AddParticipant(Guid userId)
    {
        if (_participants.Any(p => p.UserId == userId))
            return; // zaten katilmis, tekrar eklemeye gerek yok

        _participants.Add(ContestParticipant.Create(Id, userId));
    }

    public bool HasStarted(DateTime now) => now >= StartTime;
    public bool HasEnded(DateTime now) => now > EndTime;
    public bool IsActive(DateTime now) => HasStarted(now) && !HasEnded(now);
}
