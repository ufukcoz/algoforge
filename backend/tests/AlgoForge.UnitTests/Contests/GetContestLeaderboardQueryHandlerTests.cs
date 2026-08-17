using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Contests.Queries.GetContestLeaderboard;
using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlgoForge.UnitTests.Contests;

public class GetContestLeaderboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRejectUnauthorizedUserForPrivateContest()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var attacker = User.Create(
            "attacker",
            "attacker@example.com",
            "hash");

        var contest = Contest.Create(
            "Private Contest",
            "Private contest",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            creator.Id,
            false,
            "SECRET12");

        contest.AddParticipant(creator.Id);

        context.Users.AddRange(creator, attacker);
        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestLeaderboardQueryHandler(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(
                new GetContestLeaderboardQuery(
                    contest.Id,
                    attacker.Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldAllowCreatorToViewPrivateContestLeaderboard()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var contest = Contest.Create(
            "Private Contest",
            "Private contest",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            creator.Id,
            false,
            "CREATOR1");

        contest.AddParticipant(creator.Id);

        context.Users.Add(creator);
        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetContestLeaderboardQuery(
                contest.Id,
                creator.Id),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("creator", result[0].Username);
        Assert.Equal(0, result[0].TotalPoints);
        Assert.Equal(0, result[0].SolvedCount);
    }

    [Fact]
    public async Task Handle_ShouldAllowParticipantToViewPrivateContestLeaderboard()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var participant = User.Create(
            "participant",
            "participant@example.com",
            "hash");

        var contest = Contest.Create(
            "Private Contest",
            "Private contest",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            creator.Id,
            false,
            "PARTICIP1");

        contest.AddParticipant(creator.Id);
        contest.AddParticipant(participant.Id);

        context.Users.AddRange(
            creator,
            participant);

        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetContestLeaderboardQuery(
                contest.Id,
                participant.Id),
            CancellationToken.None);

        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            x => x.Username == "creator");

        Assert.Contains(
            result,
            x => x.Username == "participant");
    }

    [Fact]
    public async Task Handle_ShouldAllowAnyoneToViewPublicContestLeaderboard()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var visitor = User.Create(
            "visitor",
            "visitor@example.com",
            "hash");

        var contest = Contest.Create(
            "Public Contest",
            "Public contest",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            creator.Id,
            true,
            null);

        contest.AddParticipant(creator.Id);

        context.Users.AddRange(
            creator,
            visitor);

        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestLeaderboardQueryHandler(context);

        var result = await handler.Handle(
            new GetContestLeaderboardQuery(
                contest.Id,
                visitor.Id),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("creator", result[0].Username);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenContestDoesNotExist()
    {
        await using var context = new TestDbContext();

        var user = User.Create(
            "user",
            "user@example.com",
            "hash");

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var handler = new GetContestLeaderboardQueryHandler(context);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(
                new GetContestLeaderboardQuery(
                    Guid.NewGuid(),
                    user.Id),
                CancellationToken.None));
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

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
