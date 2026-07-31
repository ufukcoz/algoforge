using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<Question> Questions { get; }
    DbSet<TestCase> TestCases { get; }
    DbSet<Submission> Submissions { get; }
    DbSet<Contest> Contests { get; }
    DbSet<ContestQuestion> ContestQuestions { get; }
    DbSet<ContestParticipant> ContestParticipants { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
