using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class TestCase : BaseEntity
{
    public Guid QuestionId { get; private set; }
    public string Input { get; private set; } = default!;
    public string ExpectedOutput { get; private set; } = default!;
    // Hidden test case'ler kullanıcıya gösterilmez, sadece judge (Sprint 4) tarafından kullanılır.
    public bool IsHidden { get; private set; }

    private TestCase() { }

    public static TestCase Create(Guid questionId, string input, string expectedOutput, bool isHidden)
    {
        return new TestCase
        {
            QuestionId = questionId,
            Input = input,
            ExpectedOutput = expectedOutput,
            IsHidden = isHidden,
        };
    }
}
