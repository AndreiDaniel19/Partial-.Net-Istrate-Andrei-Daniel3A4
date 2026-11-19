namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default);

    // R9: active bookings of one participant, across all rooms.
    Task<List<Booking>> GetActiveForBookedByAsync(string bookedBy, CancellationToken cancellationToken = default);

    void Add(Booking booking);
}
