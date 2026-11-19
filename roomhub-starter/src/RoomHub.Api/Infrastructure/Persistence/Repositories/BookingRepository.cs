using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

// Temporary (Level A): in-memory data, lost on restart. Replaced by EF Core + SQLite at Level C.
public class BookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public void Add(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }

    public Booking? Get(Guid id)
    {
        lock (_lock)
        {
            return _bookings.FirstOrDefault(b => b.Id == id);
        }
    }

    public List<Booking> ListForRoom(Guid roomId)
    {
        lock (_lock)
        {
            return _bookings.Where(b => b.RoomId == roomId).ToList();
        }
    }
}
