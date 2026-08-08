using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Questions.Queries.GetQuestionById;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.UnitTests.Questions;

public class GetQuestionByIdQueryHandlerTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldReturnQuestion_WhenQuestionExists()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        var question = Question.Create(
            "Binary Search",
            Difficulty.Easy,
            "Find an element using binary search.",
            category.Id,
            2000,
            256);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var handler = new GetQuestionByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionByIdQuery(question.Id),
            CancellationToken.None);

        result.Should().NotBeNull();

        result!.Id.Should().Be(question.Id);
        result.Title.Should().Be("Binary Search");
        result.Difficulty.Should().Be("Easy");
        result.Description.Should().Be(
            "Find an element using binary search.");

        result.TimeLimitMs.Should().Be(2000);
        result.MemoryLimitMb.Should().Be(256);
        result.CategoryName.Should().Be("Algorithms");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenQuestionDoesNotExist()
    {
        await using var context = CreateContext();

        var handler = new GetQuestionByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyVisibleTestCases()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        var question = Question.Create(
            "Two Sum",
            Difficulty.Easy,
            "Find two numbers whose sum equals target.",
            category.Id);

        question.AddTestCase(
            "4\n2 7 11 15\n9",
            "0 1",
            false);

        question.AddTestCase(
            "4\n1 8 6 2\n10",
            "0 2",
            true);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var handler = new GetQuestionByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionByIdQuery(question.Id),
            CancellationToken.None);

        result.Should().NotBeNull();

        result!.ExampleTestCases.Should().HaveCount(1);

        result.ExampleTestCases[0].Input
            .Should()
            .Be("4\n2 7 11 15\n9");

        result.ExampleTestCases[0].ExpectedOutput
            .Should()
            .Be("0 1");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyTestCases_WhenQuestionHasNoTestCases()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        var question = Question.Create(
            "Reverse Text",
            Difficulty.Easy,
            "Reverse the given text.",
            category.Id);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var handler = new GetQuestionByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionByIdQuery(question.Id),
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.ExampleTestCases.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllVisibleTestCases()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        var question = Question.Create(
            "Maximum Subarray",
            Difficulty.Medium,
            "Find the maximum subarray sum.",
            category.Id);

        question.AddTestCase(
            "5\n-2 1 -3 4 -1 2 1 -5 4",
            "6",
            false);

        question.AddTestCase(
            "4\n2 3 -2 5",
            "8",
            false);

        question.AddTestCase(
            "3\n-1 -2 -3",
            "-1",
            true);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var handler = new GetQuestionByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetQuestionByIdQuery(question.Id),
            CancellationToken.None);

        result.Should().NotBeNull();

        result!.ExampleTestCases.Should().HaveCount(2);

        result.ExampleTestCases[0].ExpectedOutput
            .Should()
            .Be("6");

        result.ExampleTestCases[1].ExpectedOutput
            .Should()
            .Be("8");
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