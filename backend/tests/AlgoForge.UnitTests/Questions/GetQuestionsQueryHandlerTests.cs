using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Questions.Queries.GetQuestions;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.UnitTests.Questions;

public class GetQuestionsQueryHandlerTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldReturnQuestionsWithPagination()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        context.Questions.AddRange(
            Question.Create(
                "Binary Search",
                Difficulty.Easy,
                "Find an element using binary search.",
                category.Id),

            Question.Create(
                "Two Sum",
                Difficulty.Easy,
                "Find two numbers whose sum equals target.",
                category.Id),

            Question.Create(
                "Maximum Subarray",
                Difficulty.Medium,
                "Find the maximum subarray sum.",
                category.Id)
        );

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(Page: 1, PageSize: 2),
            CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnQuestionsOrderedByTitle()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        context.Questions.AddRange(
            Question.Create(
                "Zebra Problem",
                Difficulty.Easy,
                "Test",
                category.Id),

            Question.Create(
                "Binary Search",
                Difficulty.Easy,
                "Test",
                category.Id),

            Question.Create(
                "Array Problem",
                Difficulty.Medium,
                "Test",
                category.Id)
        );

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(),
            CancellationToken.None);

        result.Items.Should().HaveCount(3);

        result.Items[0].Title.Should().Be("Array Problem");
        result.Items[1].Title.Should().Be("Binary Search");
        result.Items[2].Title.Should().Be("Zebra Problem");
    }

    [Fact]
    public async Task Handle_ShouldFilterByCategory()
    {
        await using var context = CreateContext();

        var algorithms = Category.Create("Algorithms");
        var dataStructures = Category.Create("Data Structures");

        context.Categories.AddRange(
            algorithms,
            dataStructures);

        context.Questions.AddRange(
            Question.Create(
                "Binary Search",
                Difficulty.Easy,
                "Test",
                algorithms.Id),

            Question.Create(
                "Linked List",
                Difficulty.Easy,
                "Test",
                dataStructures.Id)
        );

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(CategoryId: algorithms.Id),
            CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();

        result.Items[0].Title.Should().Be("Binary Search");
        result.Items[0].CategoryName.Should().Be("Algorithms");
    }

    [Fact]
    public async Task Handle_ShouldFilterByDifficulty()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        context.Questions.AddRange(
            Question.Create(
                "Easy Question",
                Difficulty.Easy,
                "Test",
                category.Id),

            Question.Create(
                "Medium Question",
                Difficulty.Medium,
                "Test",
                category.Id)
        );

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(Difficulty: Difficulty.Medium),
            CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();

        result.Items[0].Title.Should().Be("Medium Question");
        result.Items[0].Difficulty.Should().Be("Medium");
    }

    [Fact]
    public async Task Handle_ShouldUsePageOne_WhenPageIsLessThanOne()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        context.Questions.Add(
            Question.Create(
                "Binary Search",
                Difficulty.Easy,
                "Test",
                category.Id));

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(Page: 0),
            CancellationToken.None);

        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultPageSize_WhenPageSizeIsInvalid()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        for (var i = 0; i < 25; i++)
        {
            context.Questions.Add(
                Question.Create(
                    $"Question {i:D2}",
                    Difficulty.Easy,
                    "Test",
                    category.Id));
        }

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(PageSize: 0),
            CancellationToken.None);

        result.PageSize.Should().Be(20);
        result.Items.Should().HaveCount(20);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultPageSize_WhenPageSizeIsGreaterThan100()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        for (var i = 0; i < 25; i++)
        {
            context.Questions.Add(
                Question.Create(
                    $"Question {i:D2}",
                    Difficulty.Easy,
                    "Test",
                    category.Id));
        }

        await context.SaveChangesAsync();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(PageSize: 101),
            CancellationToken.None);

        result.PageSize.Should().Be(20);
        result.Items.Should().HaveCount(20);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoQuestionsExist()
    {
        await using var context = CreateContext();

        var handler = new GetQuestionsQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionsQuery(),
            CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    private sealed class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<TestCase> TestCases => Set<TestCase>();
        public DbSet<Submission> Submissions => Set<Submission>();
        public DbSet<Contest> Contests => Set<Contest>();
        public DbSet<ContestQuestion> ContestQuestions => Set<ContestQuestion>();
        public DbSet<ContestParticipant> ContestParticipants => Set<ContestParticipant>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

        async Task<int> IApplicationDbContext.SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}