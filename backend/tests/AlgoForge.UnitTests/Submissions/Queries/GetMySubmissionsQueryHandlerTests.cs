using AlgoForge.Application.Submissions.Queries.GetMySubmissions;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlgoForge.UnitTests.Submissions.Queries;

public class GetMySubmissionsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyCurrentUsersSubmissions()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Two Sum",
            Difficulty.Easy,
            "Find two numbers.",
            category.Id);

        var user1 = User.Create(
            "user1",
            "user1@example.com",
            "hash");

        var user2 = User.Create(
            "user2",
            "user2@example.com",
            "hash");

        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Users.AddRange(user1, user2);

        var submission1 = Submission.Create(
            user1.Id,
            question.Id,
            "csharp",
            "code 1");

        submission1.MarkResult(
            SubmissionStatus.Accepted,
            100,
            500);

        var submission2 = Submission.Create(
            user2.Id,
            question.Id,
            "python",
            "code 2");

        submission2.MarkResult(
            SubmissionStatus.WrongAnswer,
            200,
            600);

        context.Submissions.AddRange(
            submission1,
            submission2);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var result = await handler.Handle(
            new GetMySubmissionsQuery(user1.Id),
            CancellationToken.None);

        Assert.Single(result);

        Assert.Equal(
            submission1.Id,
            result[0].Id);

        Assert.Equal(
            "Two Sum",
            result[0].QuestionTitle);

        Assert.Equal(
            "csharp",
            result[0].Language);

        Assert.Equal(
            "Accepted",
            result[0].Status);

        Assert.Equal(
            100,
            result[0].RuntimeMs);

        Assert.Equal(
            500,
            result[0].MemoryKb);
    }

    [Fact]
    public async Task Handle_ShouldNeverReturnAnotherUsersSubmissions()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Security");

        var question = Question.Create(
            "Authorization Test",
            Difficulty.Easy,
            "IDOR test question.",
            category.Id);

        var userA = User.Create(
            "userA",
            "userA@example.com",
            "hash");

        var userB = User.Create(
            "userB",
            "userB@example.com",
            "hash");

        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Users.AddRange(userA, userB);

        var userASubmission = Submission.Create(
            userA.Id,
            question.Id,
            "csharp",
            "user A code");

        userASubmission.MarkResult(
            SubmissionStatus.Accepted,
            100,
            200);

        var userBSubmission = Submission.Create(
            userB.Id,
            question.Id,
            "python",
            "user B code");

        userBSubmission.MarkResult(
            SubmissionStatus.Accepted,
            120,
            220);

        context.Submissions.AddRange(
            userASubmission,
            userBSubmission);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var userAResult = await handler.Handle(
            new GetMySubmissionsQuery(userA.Id),
            CancellationToken.None);

        var userBResult = await handler.Handle(
            new GetMySubmissionsQuery(userB.Id),
            CancellationToken.None);

        Assert.Single(userAResult);
        Assert.Single(userBResult);

        Assert.Equal(
            userASubmission.Id,
            userAResult[0].Id);

        Assert.Equal(
            userBSubmission.Id,
            userBResult[0].Id);

        Assert.DoesNotContain(
            userAResult,
            x => x.Id == userBSubmission.Id);

        Assert.DoesNotContain(
            userBResult,
            x => x.Id == userASubmission.Id);
    }

    [Fact]
    public async Task Handle_ShouldFilterByQuestionId()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        var question1 = Question.Create(
            "Two Sum",
            Difficulty.Easy,
            "Question one.",
            category.Id);

        var question2 = Question.Create(
            "Binary Search",
            Difficulty.Medium,
            "Question two.",
            category.Id);

        var user = User.Create(
            "testuser",
            "test@example.com",
            "hash");

        context.Categories.Add(category);
        context.Questions.AddRange(question1, question2);
        context.Users.Add(user);

        var submission1 = Submission.Create(
            user.Id,
            question1.Id,
            "csharp",
            "code 1");

        submission1.MarkResult(
            SubmissionStatus.Accepted,
            100,
            200);

        var submission2 = Submission.Create(
            user.Id,
            question2.Id,
            "csharp",
            "code 2");

        submission2.MarkResult(
            SubmissionStatus.WrongAnswer,
            150,
            250);

        context.Submissions.AddRange(
            submission1,
            submission2);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var result = await handler.Handle(
            new GetMySubmissionsQuery(
                user.Id,
                question2.Id),
            CancellationToken.None);

        Assert.Single(result);

        Assert.Equal(
            submission2.Id,
            result[0].Id);

        Assert.Equal(
            "Binary Search",
            result[0].QuestionTitle);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllUserSubmissions_WhenQuestionFilterIsNotProvided()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        var question1 = Question.Create(
            "Question One",
            Difficulty.Easy,
            "First question.",
            category.Id);

        var question2 = Question.Create(
            "Question Two",
            Difficulty.Hard,
            "Second question.",
            category.Id);

        var user = User.Create(
            "testuser",
            "test@example.com",
            "hash");

        context.Categories.Add(category);
        context.Questions.AddRange(question1, question2);
        context.Users.Add(user);

        var submission1 = Submission.Create(
            user.Id,
            question1.Id,
            "csharp",
            "code 1");

        submission1.MarkResult(
            SubmissionStatus.Accepted,
            100,
            200);

        var submission2 = Submission.Create(
            user.Id,
            question2.Id,
            "python",
            "code 2");

        submission2.MarkResult(
            SubmissionStatus.CompileError,
            null,
            null);

        context.Submissions.AddRange(
            submission1,
            submission2);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var result = await handler.Handle(
            new GetMySubmissionsQuery(user.Id),
            CancellationToken.None);

        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            x => x.Id == submission1.Id);

        Assert.Contains(
            result,
            x => x.Id == submission2.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoSubmissions()
    {
        await using var context = new TestDbContext();

        var user = User.Create(
            "emptyuser",
            "empty@example.com",
            "hash");

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var result = await handler.Handle(
            new GetMySubmissionsQuery(user.Id),
            CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldOrderByCreatedAtDescending()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Sorting",
            Difficulty.Medium,
            "Sorting question.",
            category.Id);

        var user = User.Create(
            "sortuser",
            "sort@example.com",
            "hash");

        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Users.Add(user);

        var olderSubmission = Submission.Create(
            user.Id,
            question.Id,
            "csharp",
            "old code");

        olderSubmission.MarkResult(
            SubmissionStatus.WrongAnswer,
            100,
            200);

        var newerSubmission = Submission.Create(
            user.Id,
            question.Id,
            "csharp",
            "new code");

        newerSubmission.MarkResult(
            SubmissionStatus.Accepted,
            80,
            150);

        context.Submissions.AddRange(
            olderSubmission,
            newerSubmission);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var result = await handler.Handle(
            new GetMySubmissionsQuery(user.Id),
            CancellationToken.None);

        Assert.Equal(2, result.Count);

        Assert.True(
            result[0].CreatedAt >= result[1].CreatedAt);
    }

    [Fact]
    public async Task Handle_ShouldMapSubmissionFieldsCorrectly()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");

        var question = Question.Create(
            "Resource Test",
            Difficulty.Hard,
            "Test question.",
            category.Id);

        var user = User.Create(
            "mappinguser",
            "mapping@example.com",
            "hash");

        context.Categories.Add(category);
        context.Questions.Add(question);
        context.Users.Add(user);

        var submission = Submission.Create(
            user.Id,
            question.Id,
            "java",
            "System.out.println();");

        submission.MarkResult(
            SubmissionStatus.TimeLimitExceeded,
            2500,
            1024);

        context.Submissions.Add(submission);

        await context.SaveChangesAsync();

        var handler = new GetMySubmissionsQueryHandler(context);

        var result = await handler.Handle(
            new GetMySubmissionsQuery(user.Id),
            CancellationToken.None);

        Assert.Single(result);

        var dto = result[0];

        Assert.Equal(submission.Id, dto.Id);
        Assert.Equal("Resource Test", dto.QuestionTitle);
        Assert.Equal("java", dto.Language);
        Assert.Equal("TimeLimitExceeded", dto.Status);
        Assert.Equal(2500, dto.RuntimeMs);
        Assert.Equal(1024, dto.MemoryKb);
        Assert.NotEqual(default, dto.CreatedAt);
    }

    private sealed class TestDbContext : DbContext,
        AlgoForge.Application.Common.Interfaces.IApplicationDbContext
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
