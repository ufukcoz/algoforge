using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<TestCase> TestCases => Set<TestCase>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Contest> Contests => Set<Contest>();
    public DbSet<ContestQuestion> ContestQuestions => Set<ContestQuestion>();
    public DbSet<ContestParticipant> ContestParticipants => Set<ContestParticipant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}
