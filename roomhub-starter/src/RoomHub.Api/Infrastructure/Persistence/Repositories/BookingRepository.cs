using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

// Temporary (Level B): in-memory data, lost on restart. Replaced by EF Core + SQLite at Level C.
public sealed class BookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_bookings.FirstOrDefault(b => b.Id == id));
        }
    }

    public Task<List<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_bookings.Where(b => b.RoomId == roomId && b.IsActive).ToList());
        }
    }

    public void Add(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }
}
