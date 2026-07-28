using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlgoForge.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    // Vizyon dokumanindaki kategori listesiyle birebir eslesen sabit GUID'ler,
    // boylece migration tekrar calistirilsa da ayni ID'ler uretilir.
    public static readonly Guid ArrayId = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid GraphId = new("00000000-0000-0000-0000-000000000002");
    public static readonly Guid TreeId = new("00000000-0000-0000-0000-000000000003");
    public static readonly Guid HashMapId = new("00000000-0000-0000-0000-000000000004");
    public static readonly Guid QueueId = new("00000000-0000-0000-0000-000000000005");
    public static readonly Guid StackId = new("00000000-0000-0000-0000-000000000006");
    public static readonly Guid StringId = new("00000000-0000-0000-0000-000000000007");
    public static readonly Guid MathId = new("00000000-0000-0000-0000-000000000008");
    public static readonly Guid DpId = new("00000000-0000-0000-0000-000000000009");
    public static readonly Guid GreedyId = new("00000000-0000-0000-0000-00000000000a");
    public static readonly Guid SortingId = new("00000000-0000-0000-0000-00000000000b");
    public static readonly Guid SearchingId = new("00000000-0000-0000-0000-00000000000c");
    public static readonly Guid BacktrackingId = new("00000000-0000-0000-0000-00000000000d");
    public static readonly Guid BitManipulationId = new("00000000-0000-0000-0000-00000000000e");

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Name).IsUnique();

        // Iliskili Questions koleksiyonu EF tarafindan otomatik yonetiliyor, doğrudan mapping gerekmiyor.
        builder.Ignore(c => c.Questions);

        builder.HasData(
            CreateSeed(ArrayId, "Array"),
            CreateSeed(GraphId, "Graph"),
            CreateSeed(TreeId, "Tree"),
            CreateSeed(HashMapId, "HashMap"),
            CreateSeed(QueueId, "Queue"),
            CreateSeed(StackId, "Stack"),
            CreateSeed(StringId, "String"),
            CreateSeed(MathId, "Math"),
            CreateSeed(DpId, "DP"),
            CreateSeed(GreedyId, "Greedy"),
            CreateSeed(SortingId, "Sorting"),
            CreateSeed(SearchingId, "Searching"),
            CreateSeed(BacktrackingId, "Backtracking"),
            CreateSeed(BitManipulationId, "Bit Manipulation")
        );
    }

    // Category.Create() private constructor + factory kullaniyor, HasData icin
    // reflection gerektirmeden anonim tip uzerinden seed veriyoruz.
    private static object CreateSeed(Guid id, string name) => new
    {
        Id = id,
        Name = name,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
    };
}
