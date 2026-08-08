using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Questions.Commands.CreateQuestion;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.UnitTests.Questions;

public class CreateQuestionCommandHandlerTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldCreateQuestion_WhenCategoryExists()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var command = new CreateQuestionCommand(
            "Binary Search",
            Difficulty.Easy,
            "Find an element using binary search.",
            category.Id,
            2000,
            256,
            new List<CreateTestCaseDto>
            {
                new(
                    "5\n1 2 3 4 5\n3",
                    "2",
                    false)
            });

        var handler = new CreateQuestionCommandHandler(context);

        var questionId = await handler.Handle(
            command,
            CancellationToken.None);

        questionId.Should().NotBeEmpty();

        var question = await context.Questions
            .Include(q => q.TestCases)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        question.Should().NotBeNull();

        question!.Title.Should().Be("Binary Search");
        question.Difficulty.Should().Be(Difficulty.Easy);
        question.Description.Should()
            .Be("Find an element using binary search.");

        question.CategoryId.Should().Be(category.Id);

        question.TimeLimitMs.Should().Be(2000);
        question.MemoryLimitMb.Should().Be(256);

        question.TestCases.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldCreateAllTestCases()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var command = new CreateQuestionCommand(
            "Two Sum",
            Difficulty.Easy,
            "Find two numbers that add up to a target.",
            category.Id,
            1500,
            128,
            new List<CreateTestCaseDto>
            {
                new(
                    "4\n2 7 11 15\n9",
                    "0 1",
                    false),

                new(
                    "3\n3 2 4\n6",
                    "1 2",
                    false),

                new(
                    "2\n3 3\n6",
                    "0 1",
                    true)
            });

        var handler = new CreateQuestionCommandHandler(context);

        var questionId = await handler.Handle(
            command,
            CancellationToken.None);

        var question = await context.Questions
            .Include(q => q.TestCases)
            .FirstAsync(q => q.Id == questionId);

        question.TestCases.Should().HaveCount(3);

        question.TestCases
            .Count(tc => tc.IsHidden)
            .Should()
            .Be(1);

        question.TestCases
            .Count(tc => !tc.IsHidden)
            .Should()
            .Be(2);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCategoryDoesNotExist()
    {
        await using var context = CreateContext();

        var command = new CreateQuestionCommand(
            "Binary Search",
            Difficulty.Easy,
            "Test description.",
            Guid.NewGuid(),
            2000,
            256,
            new List<CreateTestCaseDto>
            {
                new(
                    "1 2 3",
                    "2",
                    false)
            });

        var handler = new CreateQuestionCommandHandler(context);

        var act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Belirtilen kategori bulunamadi.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoTestCasesProvided()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var command = new CreateQuestionCommand(
            "Binary Search",
            Difficulty.Easy,
            "Test description.",
            category.Id,
            2000,
            256,
            new List<CreateTestCaseDto>());

        var handler = new CreateQuestionCommandHandler(context);

        var act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("En az bir test case gerekli.");
    }

    [Fact]
    public async Task Handle_ShouldReturnCreatedQuestionId()
    {
        await using var context = CreateContext();

        var category = Category.Create("Data Structures");
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var command = new CreateQuestionCommand(
            "Linked List",
            Difficulty.Medium,
            "Implement a linked list.",
            category.Id,
            3000,
            512,
            new List<CreateTestCaseDto>
            {
                new(
                    "1 2 3",
                    "3 2 1",
                    false)
            });

        var handler = new CreateQuestionCommandHandler(context);

        var questionId = await handler.Handle(
            command,
            CancellationToken.None);

        questionId.Should().NotBeEmpty();

        var exists = await context.Questions
            .AnyAsync(q => q.Id == questionId);

        exists.Should().BeTrue();
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