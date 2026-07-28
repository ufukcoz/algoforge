using AlgoForge.Domain.Common;

namespace AlgoForge.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;

    private readonly List<Question> _questions = new();
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    private Category() { }

    public static Category Create(string name)
    {
        return new Category { Name = name };
    }
}
