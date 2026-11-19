using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Infrastructure.Persistence;

// Temporary (Level B): the in-memory repositories keep entity references, so there is nothing to save.
// Replaced by the EF Core DbContext (which implements IUnitOfWork) at Level C.
public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
}
