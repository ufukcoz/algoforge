using AlgoForge.Application.Categories.Queries.GetCategories;
using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlgoForge.UnitTests.Categories;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCategoriesOrderedByName()
    {
        await using var context = new TestDbContext();

        context.Categories.Add(Category.Create("Zoology"));
        context.Categories.Add(Category.Create("Algorithms"));
        context.Categories.Add(Category.Create("Databases"));

        await context.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(context);

        var result = await handler.Handle(
            new GetCategoriesQuery(),
            CancellationToken.None);

        Assert.Equal(3, result.Count);

        Assert.Equal("Algorithms", result[0].Name);
        Assert.Equal("Databases", result[1].Name);
        Assert.Equal("Zoology", result[2].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectQuestionCount()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        var question1 = Question.Create(
            "Two Sum",
            AlgoForge.Domain.Enums.Difficulty.Easy,
            "Find two numbers.",
            category.Id);

        var question2 = Question.Create(
            "Binary Search",
            AlgoForge.Domain.Enums.Difficulty.Medium,
            "Search an array.",
            category.Id);

        context.Questions.Add(question1);
        context.Questions.Add(question2);

        await context.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(context);

        var result = await handler.Handle(
            new GetCategoriesQuery(),
            CancellationToken.None);

        var categoryResult = Assert.Single(result);

        Assert.Equal("Algorithms", categoryResult.Name);
        Assert.Equal(2, categoryResult.QuestionCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnZeroQuestionCount_WhenCategoryHasNoQuestions()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(context);

        var result = await handler.Handle(
            new GetCategoriesQuery(),
            CancellationToken.None);

        var categoryResult = Assert.Single(result);

        Assert.Equal("Algorithms", categoryResult.Name);
        Assert.Equal(0, categoryResult.QuestionCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenThereAreNoCategories()
    {
        await using var context = new TestDbContext();

        var handler = new GetCategoriesQueryHandler(context);

        var result = await handler.Handle(
            new GetCategoriesQuery(),
            CancellationToken.None);

        Assert.Empty(result);
    }

    private sealed class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext()
            : base(
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options)
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
    }
}