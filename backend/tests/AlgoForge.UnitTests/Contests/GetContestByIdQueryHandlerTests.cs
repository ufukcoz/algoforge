using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Contests.Queries.GetContestById;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlgoForge.UnitTests.Contests;

public class GetContestByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenContestDoesNotExist()
    {
        await using var context = new TestDbContext();

        var handler = new GetContestByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetContestByIdQuery(
                Guid.NewGuid(),
                Guid.NewGuid()),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldAllowAccessToPublicContest()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Two Sum",
            Difficulty.Easy,
            "Find two numbers.",
            category.Id);

        var contest = Contest.Create(
            "Public Contest",
            "Public contest description",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            creator.Id,
            true,
            null);

        contest.AddQuestion(
            question.Id,
            100,
            0);

        context.Users.Add(creator);
        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var anotherUser = User.Create(
            "participant",
            "participant@example.com",
            "hash");

        context.Users.Add(anotherUser);
        await context.SaveChangesAsync();

        var handler = new GetContestByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetContestByIdQuery(
                contest.Id,
                anotherUser.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(contest.Id, result!.Id);
        Assert.True(result.IsPublic);
        Assert.False(result.IsJoined);
        Assert.Null(result.InviteCode);
        Assert.Single(result.Questions);
        Assert.Equal(
            "Two Sum",
            result.Questions[0].Title);
    }

    [Fact]
    public async Task Handle_ShouldAllowCreatorToAccessPrivateContest()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Binary Search",
            Difficulty.Medium,
            "Search efficiently.",
            category.Id);

        var inviteCode = "ABC23456";

        var contest = Contest.Create(
            "Private Contest",
            "Private contest description",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            creator.Id,
            false,
            inviteCode);

        contest.AddQuestion(
            question.Id,
            200,
            0);

        contest.AddParticipant(creator.Id);

        context.Users.Add(creator);
        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetContestByIdQuery(
                contest.Id,
                creator.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(contest.Id, result!.Id);
        Assert.False(result.IsPublic);
        Assert.True(result.IsJoined);
        Assert.Equal(inviteCode, result.InviteCode);
        Assert.Single(result.Questions);
    }

    [Fact]
    public async Task Handle_ShouldAllowParticipantToAccessPrivateContest()
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

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Graph Traversal",
            Difficulty.Hard,
            "Traverse a graph.",
            category.Id);

        var contest = Contest.Create(
            "Private Contest",
            "Private contest description",
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            creator.Id,
            false,
            "XYZ23456");

        contest.AddQuestion(
            question.Id,
            300,
            0);

        contest.AddParticipant(creator.Id);
        contest.AddParticipant(participant.Id);

        context.Users.AddRange(
            creator,
            participant);

        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetContestByIdQuery(
                contest.Id,
                participant.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result!.IsPublic);
        Assert.True(result.IsJoined);

        // Invite code sadece creator'a gösterilmeli.
        Assert.Null(result.InviteCode);

        Assert.Equal(
            participant.Id != creator.Id,
            result.IsJoined);
    }

    [Fact]
    public async Task Handle_ShouldRejectUnauthorizedUserForPrivateContest()
    {
        await using var context = new TestDbContext();

        var creator = User.Create(
            "creator",
            "creator@example.com",
            "hash");

        var unauthorizedUser = User.Create(
            "attacker",
            "attacker@example.com",
            "hash");

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Private Question",
            Difficulty.Easy,
            "Private question.",
            category.Id);

        var contest = Contest.Create(
            "Private Contest",
            "Sensitive contest",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            creator.Id,
            false,
            "PRIVATE1");

        contest.AddQuestion(
            question.Id,
            100,
            0);

        contest.AddParticipant(creator.Id);

        context.Users.AddRange(
            creator,
            unauthorizedUser);

        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestByIdQueryHandler(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(
                new GetContestByIdQuery(
                    contest.Id,
                    unauthorizedUser.Id),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldNotExposeInviteCodeToParticipant()
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
            "SECRET12");

        contest.AddParticipant(creator.Id);
        contest.AddParticipant(participant.Id);

        context.Users.AddRange(
            creator,
            participant);

        context.Contests.Add(contest);

        await context.SaveChangesAsync();

        var handler = new GetContestByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetContestByIdQuery(
                contest.Id,
                participant.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result!.InviteCode);
    }

    [Fact]
    public async Task Handle_ShouldReturnInviteCodeOnlyToCreator()
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

        var handler = new GetContestByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetContestByIdQuery(
                contest.Id,
                creator.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(
            "CREATOR1",
            result!.InviteCode);
    }

    private sealed class TestDbContext : DbContext,
        IApplicationDbContext
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

        public DbSet<ContestQuestion> ContestQuestions =>
            Set<ContestQuestion>();

        public DbSet<ContestParticipant> ContestParticipants =>
            Set<ContestParticipant>();

        public DbSet<RefreshToken> RefreshTokens =>
            Set<RefreshToken>();

        public DbSet<EmailVerificationToken> EmailVerificationTokens =>
            Set<EmailVerificationToken>();

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
