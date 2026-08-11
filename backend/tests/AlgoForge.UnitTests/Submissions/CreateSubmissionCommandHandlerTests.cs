using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Submissions.Commands.CreateSubmission;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlgoForge.UnitTests.Submissions;

public class CreateSubmissionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenQuestionDoesNotExist()
    {
        await using var context = new TestDbContext();
        var judge = new FakeJudgeService();
        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "csharp",
            "Console.WriteLine();"
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldRunAllTestCases_IncludingHidden()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Two Sum",
            Difficulty.Easy,
            "Find the target.",
            category.Id
        );

        question.AddTestCase("1 2", "3", false);
        question.AddTestCase("5 7", "12", true);

        context.Questions.Add(question);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    true,
                    "3",
                    null,
                    null,
                    10,
                    100,
                    "Accepted"
                ),
                new JudgeExecutionResult(
                    true,
                    "12",
                    null,
                    null,
                    20,
                    200,
                    "Accepted"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            Guid.NewGuid(),
            question.Id,
            "csharp",
            "return;"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.PassedCount);
        Assert.Equal("Accepted", result.Status);
        Assert.Equal(20, result.RuntimeMs);
        Assert.Equal(200, result.MemoryKb);

        Assert.Equal(2, judge.CallCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnWrongAnswer_WhenTestCaseFails()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Sum",
            Difficulty.Easy,
            "Calculate sum.",
            category.Id
        );

        question.AddTestCase("1 2", "3", false);

        context.Questions.Add(question);

        var user = User.Create(
            "testuser",
            "test@example.com",
            "hash"
        );

        context.Users.Add(user);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    false,
                    "4",
                    null,
                    null,
                    15,
                    120,
                    "Wrong Answer"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            user.Id,
            question.Id,
            "csharp",
            "wrong code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("WrongAnswer", result.Status);
        Assert.Equal(0, result.PassedCount);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(15, result.RuntimeMs);
        Assert.Equal(120, result.MemoryKb);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimeLimitExceeded_WhenJudgeReportsTimeout()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Slow Question",
            Difficulty.Medium,
            "Test timeout.",
            category.Id
        );

        question.AddTestCase("input", "output", false);

        context.Questions.Add(question);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    false,
                    null,
                    null,
                    null,
                    2000,
                    500,
                    "Time Limit Exceeded"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            Guid.NewGuid(),
            question.Id,
            "csharp",
            "slow code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("TimeLimitExceeded", result.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnCompileError_WhenJudgeReportsCompilationError()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Compile Test",
            Difficulty.Easy,
            "Test compilation error.",
            category.Id
        );

        question.AddTestCase("input", "output", false);

        context.Questions.Add(question);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    false,
                    null,
                    null,
                    "Compilation failed",
                    null,
                    null,
                    "Compilation Error"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            Guid.NewGuid(),
            question.Id,
            "csharp",
            "invalid code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("CompileError", result.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnRuntimeError_WhenJudgeReportsRuntimeError()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Runtime Test",
            Difficulty.Easy,
            "Test runtime error.",
            category.Id
        );

        question.AddTestCase("input", "output", false);

        context.Questions.Add(question);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    false,
                    null,
                    "Runtime failure",
                    null,
                    50,
                    100,
                    "Runtime Error (SIGSEGV)"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            Guid.NewGuid(),
            question.Id,
            "csharp",
            "crash code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("RuntimeError", result.Status);
    }

    [Fact]
    public async Task Handle_ShouldGiveXp_OnFirstAcceptedSubmission()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Easy Question",
            Difficulty.Easy,
            "XP test.",
            category.Id
        );

        question.AddTestCase("input", "output", false);

        var user = User.Create(
            "xpuser",
            "xp@example.com",
            "hash"
        );

        context.Questions.Add(question);
        context.Users.Add(user);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    true,
                    "output",
                    null,
                    null,
                    10,
                    100,
                    "Accepted"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            user.Id,
            question.Id,
            "csharp",
            "correct code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("Accepted", result.Status);
        Assert.Equal(10, user.Xp);
        Assert.Equal(1, user.Level);
    }

    [Fact]
    public async Task Handle_ShouldNotGiveXp_WhenUserAlreadySolvedQuestion()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Already Solved",
            Difficulty.Easy,
            "Already solved test.",
            category.Id
        );

        question.AddTestCase("input", "output", false);

        var user = User.Create(
            "solveduser",
            "solved@example.com",
            "hash"
        );

        context.Questions.Add(question);
        context.Users.Add(user);

        var previousSubmission = Submission.Create(
            user.Id,
            question.Id,
            "csharp",
            "previous correct code"
        );

        previousSubmission.MarkResult(
            SubmissionStatus.Accepted,
            10,
            100
        );

        context.Submissions.Add(previousSubmission);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    true,
                    "output",
                    null,
                    null,
                    20,
                    200,
                    "Accepted"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            user.Id,
            question.Id,
            "csharp",
            "another correct code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("Accepted", result.Status);
        Assert.Equal(0, user.Xp);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalError_WhenQuestionHasNoTestCases()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "No Tests",
            Difficulty.Easy,
            "No test cases.",
            category.Id
        );

        var user = User.Create(
            "notestuser",
            "notest@example.com",
            "hash"
        );

        context.Questions.Add(question);
        context.Users.Add(user);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService();

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            user.Id,
            question.Id,
            "csharp",
            "code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal("InternalError", result.Status);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.PassedCount);
        Assert.Equal(0, judge.CallCount);
    }

    [Fact]
    public async Task Handle_ShouldUseMaximumRuntimeAndMemory()
    {
        await using var context = new TestDbContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Resource Test",
            Difficulty.Hard,
            "Resource test.",
            category.Id
        );

        question.AddTestCase("1", "1", false);
        question.AddTestCase("2", "2", true);
        question.AddTestCase("3", "3", true);

        context.Questions.Add(question);

        await context.SaveChangesAsync(CancellationToken.None);

        var judge = new FakeJudgeService
        {
            Results =
            {
                new JudgeExecutionResult(
                    true,
                    "1",
                    null,
                    null,
                    100,
                    500,
                    "Accepted"
                ),
                new JudgeExecutionResult(
                    true,
                    "2",
                    null,
                    null,
                    250,
                    300,
                    "Accepted"
                ),
                new JudgeExecutionResult(
                    true,
                    "3",
                    null,
                    null,
                    150,
                    900,
                    "Accepted"
                )
            }
        };

        var handler = new CreateSubmissionCommandHandler(context, judge);

        var command = new CreateSubmissionCommand(
            Guid.NewGuid(),
            question.Id,
            "csharp",
            "code"
        );

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.PassedCount);
        Assert.Equal(250, result.RuntimeMs);
        Assert.Equal(900, result.MemoryKb);
    }

    private sealed class FakeJudgeService : IJudgeService
    {
        public List<JudgeExecutionResult> Results { get; } = new();

        public int CallCount { get; private set; }

        private int _currentIndex;

        public Task<JudgeExecutionResult> ExecuteAsync(
            string sourceCode,
            string language,
            string stdin,
            string expectedOutput,
            int timeLimitMs,
            int memoryLimitMb,
            CancellationToken cancellationToken)
        {
            CallCount++;

            if (_currentIndex >= Results.Count)
            {
                return Task.FromResult(
                    new JudgeExecutionResult(
                        true,
                        expectedOutput,
                        null,
                        null,
                        10,
                        100,
                        "Accepted"
                    )
                );
            }

            var result = Results[_currentIndex];
            _currentIndex++;

            return Task.FromResult(result);
        }
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

        public DbSet<EmailVerificationToken> EmailVerificationTokens =>
            Set<EmailVerificationToken>();

     public override Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
{
    return base.SaveChangesAsync(cancellationToken);
}
    }
}