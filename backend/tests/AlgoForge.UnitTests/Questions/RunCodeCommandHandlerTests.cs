using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Application.Questions.Commands.RunCode;
using AlgoForge.Domain.Entities;
using AlgoForge.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.UnitTests.Questions;

public class RunCodeCommandHandlerTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenQuestionDoesNotExist()
    {
        await using var context = CreateContext();

        var judge = new FakeJudgeService();
        var handler = new RunCodeCommandHandler(context, judge);

        var command = new RunCodeCommand(
            Guid.NewGuid(),
            "csharp",
            "Console.WriteLine(\"Hello\");");

        var act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Soru bulunamadi.");
    }

    [Fact]
    public async Task Handle_ShouldExecuteOnlyVisibleTestCases()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Two Sum",
            Difficulty.Easy,
            "Find two numbers.",
            category.Id);

        question.AddTestCase(
            "1 2",
            "3",
            false);

        question.AddTestCase(
            "5 10",
            "15",
            true);

        question.AddTestCase(
            "20 30",
            "50",
            false);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var judge = new FakeJudgeService
        {
            ResultFactory = (
                sourceCode,
                language,
                stdin,
                expectedOutput,
                timeLimitMs,
                memoryLimitMb) =>
                new JudgeExecutionResult(
                    true,
                    expectedOutput,
                    null,
                    null,
                    10,
                    1024,
                    "Accepted")
        };

        var handler = new RunCodeCommandHandler(context, judge);

        var command = new RunCodeCommand(
            question.Id,
            "csharp",
            "Console.WriteLine();");

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        result.Results.Should().HaveCount(2);

        judge.Executions.Should().HaveCount(2);

        judge.Executions.Should().NotContain(
            execution => execution.Stdin == "5 10");

        judge.Executions.Should().Contain(
            execution => execution.Stdin == "1 2");

        judge.Executions.Should().Contain(
            execution => execution.Stdin == "20 30");
    }

    [Fact]
    public async Task Handle_ShouldReturnAllPassedTrue_WhenAllVisibleTestsPass()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Addition",
            Difficulty.Easy,
            "Add two numbers.",
            category.Id);

        question.AddTestCase(
            "2 3",
            "5",
            false);

        question.AddTestCase(
            "10 20",
            "30",
            false);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var judge = new FakeJudgeService
        {
            ResultFactory = (
                sourceCode,
                language,
                stdin,
                expectedOutput,
                timeLimitMs,
                memoryLimitMb) =>
                new JudgeExecutionResult(
                    true,
                    expectedOutput,
                    null,
                    null,
                    5,
                    1024,
                    "Accepted")
        };

        var handler = new RunCodeCommandHandler(context, judge);

        var result = await handler.Handle(
            new RunCodeCommand(
                question.Id,
                "csharp",
                "Console.WriteLine();"),
            CancellationToken.None);

        result.AllPassed.Should().BeTrue();
        result.Results.Should().HaveCount(2);
        result.Results.Should().OnlyContain(r => r.Passed);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllPassedFalse_WhenAnyTestFails()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Addition",
            Difficulty.Easy,
            "Add two numbers.",
            category.Id);

        question.AddTestCase(
            "2 3",
            "5",
            false);

        question.AddTestCase(
            "10 20",
            "30",
            false);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var callCount = 0;

        var judge = new FakeJudgeService
        {
            ResultFactory = (
                sourceCode,
                language,
                stdin,
                expectedOutput,
                timeLimitMs,
                memoryLimitMb) =>
            {
                callCount++;

                if (callCount == 1)
                {
                    return new JudgeExecutionResult(
                        true,
                        "5",
                        null,
                        null,
                        5,
                        1024,
                        "Accepted");
                }

                return new JudgeExecutionResult(
                    false,
                    "999",
                    null,
                    null,
                    8,
                    1024,
                    "Wrong Answer");
            }
        };

        var handler = new RunCodeCommandHandler(context, judge);

        var result = await handler.Handle(
            new RunCodeCommand(
                question.Id,
                "csharp",
                "Console.WriteLine();"),
            CancellationToken.None);

        result.AllPassed.Should().BeFalse();
        result.Results.Should().HaveCount(2);

        result.Results[0].Passed.Should().BeTrue();
        result.Results[1].Passed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenThereAreNoVisibleTestCases()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Hidden Only",
            Difficulty.Medium,
            "Question with only hidden tests.",
            category.Id);

        question.AddTestCase(
            "secret input",
            "secret output",
            true);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var judge = new FakeJudgeService();

        var handler = new RunCodeCommandHandler(context, judge);

        var result = await handler.Handle(
            new RunCodeCommand(
                question.Id,
                "csharp",
                "Console.WriteLine();"),
            CancellationToken.None);

        result.AllPassed.Should().BeFalse();
        result.Results.Should().BeEmpty();
        judge.Executions.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapJudgeResultCorrectly()
    {
        await using var context = CreateContext();

        var category = Category.Create("Algorithms");
        context.Categories.Add(category);

        var question = Question.Create(
            "Test Question",
            Difficulty.Easy,
            "Test.",
            category.Id,
            2000,
            256);

        question.AddTestCase(
            "input",
            "expected",
            false);

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var judge = new FakeJudgeService
        {
            ResultFactory = (
                sourceCode,
                language,
                stdin,
                expectedOutput,
                timeLimitMs,
                memoryLimitMb) =>
                new JudgeExecutionResult(
                    false,
                    "actual output",
                    "runtime error",
                    "compile output",
                    123,
                    2048,
                    "Runtime Error")
        };

        var handler = new RunCodeCommandHandler(context, judge);

        var result = await handler.Handle(
            new RunCodeCommand(
                question.Id,
                "python",
                "print('test')"),
            CancellationToken.None);

        result.AllPassed.Should().BeFalse();

        var testResult = result.Results
            .Should()
            .ContainSingle()
            .Subject;

        testResult.Input.Should().Be("input");
        testResult.ExpectedOutput.Should().Be("expected");
        testResult.ActualOutput.Should().Be("actual output");
        testResult.Passed.Should().BeFalse();
        testResult.Stderr.Should().Be("runtime error");
        testResult.CompileOutput.Should().Be("compile output");
        testResult.RuntimeMs.Should().Be(123);
    }

    private sealed class FakeJudgeService : IJudgeService
    {
        public List<JudgeExecution> Executions { get; } = new();

        public Func<
            string,
            string,
            string,
            string,
            int,
            int,
            JudgeExecutionResult>? ResultFactory { get; set; }

        public Task<JudgeExecutionResult> ExecuteAsync(
            string sourceCode,
            string language,
            string stdin,
            string expectedOutput,
            int timeLimitMs,
            int memoryLimitMb,
            CancellationToken cancellationToken)
        {
            Executions.Add(
                new JudgeExecution(
                    sourceCode,
                    language,
                    stdin,
                    expectedOutput,
                    timeLimitMs,
                    memoryLimitMb));

            var result = ResultFactory?.Invoke(
                sourceCode,
                language,
                stdin,
                expectedOutput,
                timeLimitMs,
                memoryLimitMb)
                ?? new JudgeExecutionResult(
                    true,
                    expectedOutput,
                    null,
                    null,
                    10,
                    1024,
                    "Accepted");

            return Task.FromResult(result);
        }
    }

    private sealed record JudgeExecution(
        string SourceCode,
        string Language,
        string Stdin,
        string ExpectedOutput,
        int TimeLimitMs,
        int MemoryLimitMb);

    private sealed class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(
            DbContextOptions<TestDbContext> options)
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