using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<Booking>;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, Booking>
{
    public async Task<Result<Booking>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // 404 before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var activeBookings = await bookings.GetActiveForRoomAsync(room.Id, cancellationToken);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var result = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            activeBookings,
            nowUtc);

        if (result.IsFailure)
            return result.Error;

        bookings.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value;
    }
}
