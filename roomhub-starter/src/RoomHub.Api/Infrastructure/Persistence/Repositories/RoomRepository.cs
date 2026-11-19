using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

// Never calls SaveChangesAsync: that is the job of the Unit of Work (the DbContext).
public sealed class RoomRepository(RoomHubDbContext dbContext) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Rooms.FindAsync([id], cancellationToken);

    public void Add(Room room) => dbContext.Rooms.Add(room);
}
