using AlgoForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
