using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

// Temporary (Level B): in-memory data, lost on restart. Replaced by EF Core + SQLite at Level C.
public sealed class RoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new();
    private readonly object _lock = new();

    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
        }
    }

    public void Add(Room room)
    {
        lock (_lock)
        {
            _rooms.Add(room);
        }
    }
}
