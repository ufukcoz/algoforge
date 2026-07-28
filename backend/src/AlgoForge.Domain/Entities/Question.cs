using AlgoForge.Domain.Common;
using AlgoForge.Domain.Enums;

namespace AlgoForge.Domain.Entities;

public class Question : BaseEntity
{
    public string Title { get; private set; } = default!;
    public Difficulty Difficulty { get; private set; }
    public string Description { get; private set; } = default!;
    public int TimeLimitMs { get; private set; }
    public int MemoryLimitMb { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;

    private readonly List<TestCase> _testCases = new();
    public IReadOnlyCollection<TestCase> TestCases => _testCases.AsReadOnly();

    private Question() { }

    public static Question Create(
        string title,
        Difficulty difficulty,
        string description,
        Guid categoryId,
        int timeLimitMs = 2000,
        int memoryLimitMb = 256)
    {
        return new Question
        {
            Title = title,
            Difficulty = difficulty,
            Description = description,
            CategoryId = categoryId,
            TimeLimitMs = timeLimitMs,
            MemoryLimitMb = memoryLimitMb,
        };
    }

    public void AddTestCase(string input, string expectedOutput, bool isHidden)
    {
        _testCases.Add(TestCase.Create(Id, input, expectedOutput, isHidden));
    }
}
