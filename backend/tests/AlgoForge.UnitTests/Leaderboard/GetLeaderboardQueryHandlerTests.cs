using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Leaderboard.Queries.GetLeaderboard;
using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlgoForge.UnitTests.Leaderboard;

public class GetLeaderboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUsersOrderedByXpDescending()
    {
        var context = new TestDbContext();

        var lowXpUser = User.Create(
            "lowuser",
            "low@example.com",
            "hash");

        var highXpUser = User.Create(
            "highuser",
            "high@example.com",
            "hash");

        var mediumXpUser = User.Create(
            "mediumuser",
            "medium@example.com",
            "hash");

        highXpUser.AddXp(300);
        mediumXpUser.AddXp(200);
        lowXpUser.AddXp(100);

        context.Users.AddRange(
            lowXpUser,
            highXpUser,
            mediumXpUser);

        await context.SaveChangesAsync();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(),
            CancellationToken.None);

        Assert.Equal(3, result.Count);

        Assert.Equal("highuser", result[0].Username);
        Assert.Equal(300, result[0].Xp);
        Assert.Equal(1, result[0].Rank);

        Assert.Equal("mediumuser", result[1].Username);
        Assert.Equal(200, result[1].Xp);
        Assert.Equal(2, result[1].Rank);

        Assert.Equal("lowuser", result[2].Username);
        Assert.Equal(100, result[2].Xp);
        Assert.Equal(3, result[2].Rank);
    }

    [Fact]
    public async Task Handle_ShouldAssignSequentialRanks()
    {
        var context = new TestDbContext();

        var user1 = User.Create(
            "user1",
            "user1@example.com",
            "hash");

        var user2 = User.Create(
            "user2",
            "user2@example.com",
            "hash");

        var user3 = User.Create(
            "user3",
            "user3@example.com",
            "hash");

        user1.AddXp(500);
        user2.AddXp(400);
        user3.AddXp(300);

        context.Users.AddRange(user1, user2, user3);

        await context.SaveChangesAsync();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(3),
            CancellationToken.None);

        Assert.Equal(3, result.Count);

        Assert.Equal(1, result[0].Rank);
        Assert.Equal(2, result[1].Rank);
        Assert.Equal(3, result[2].Rank);
    }

    [Fact]
    public async Task Handle_ShouldRespectTopParameter()
    {
        var context = new TestDbContext();

        for (var i = 1; i <= 10; i++)
        {
            var user = User.Create(
                $"user{i}",
                $"user{i}@example.com",
                "hash");

            user.AddXp(i * 100);

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(3),
            CancellationToken.None);

        Assert.Equal(3, result.Count);

        Assert.Equal("user10", result[0].Username);
        Assert.Equal("user9", result[1].Username);
        Assert.Equal("user8", result[2].Username);
    }

    [Fact]
    public async Task Handle_ShouldUseDefault50_WhenTopIsLessThanOne()
    {
        var context = new TestDbContext();

        for (var i = 1; i <= 60; i++)
        {
            var user = User.Create(
                $"user{i}",
                $"user{i}@example.com",
                "hash");

            user.AddXp(i);

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(0),
            CancellationToken.None);

        Assert.Equal(50, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldUseDefault50_WhenTopIsGreaterThan200()
    {
        var context = new TestDbContext();

        for (var i = 1; i <= 60; i++)
        {
            var user = User.Create(
                $"user{i}",
                $"user{i}@example.com",
                "hash");

            user.AddXp(i);

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(201),
            CancellationToken.None);

        Assert.Equal(50, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenThereAreNoUsers()
    {
        var context = new TestDbContext();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(),
            CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectLevel()
    {
        var context = new TestDbContext();

        var user = User.Create(
            "leveluser",
            "level@example.com",
            "hash");

        user.AddXp(1200);

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var handler = new GetLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetLeaderboardQuery(),
            CancellationToken.None);

        Assert.Single(result);

        Assert.Equal("leveluser", result[0].Username);
        Assert.Equal(1200, result[0].Xp);
        Assert.Equal(2, result[0].Level);
        Assert.Equal(1, result[0].Rank);
    }

    private sealed class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext()
            : base(new DbContextOptionsBuilder<TestDbContext>()
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

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}