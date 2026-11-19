using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

// Never calls SaveChangesAsync: that is the job of the Unit of Work (the DbContext).
public sealed class BookingRepository(RoomHubDbContext dbContext) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Bookings.FindAsync([id], cancellationToken);

    public Task<List<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default) =>
        dbContext.Bookings
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

    public Task<List<Booking>> GetActiveForBookedByAsync(string bookedBy, CancellationToken cancellationToken = default) =>
        dbContext.Bookings
            .Where(b => b.BookedBy == bookedBy && b.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

    public void Add(Booking booking) => dbContext.Bookings.Add(booking);
}
