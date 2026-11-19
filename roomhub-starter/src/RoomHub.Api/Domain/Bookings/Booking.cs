using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    public const int MaxActivePerParticipantPerDay = 2;

    private Booking()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateRange Range { get; private set; } = null!;
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        IEnumerable<Booking> existingRoomBookings,
        IEnumerable<Booking> existingPersonBookings,
        DateTime nowUtc)
    {
        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.BookedByRequired;

        bookedBy = bookedBy.Trim();

        // R3
        var rangeResult = DateRange.Create(startUtc, endUtc);
        if (rangeResult.IsFailure)
            return rangeResult.Error;

        var range = rangeResult.Value;

        // R4
        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        // R5
        if (range.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        // R6: exactly 4 hours is allowed.
        if (range.Duration > MaxDuration)
            return BookingErrors.TooLong;

        // R7: only active bookings of the same room block the interval.
        if (existingRoomBookings.Any(b => b.RoomId == room.Id && b.IsActive && b.Range.Overlaps(range)))
            return BookingErrors.Overlap;

        // R9: at most 2 active bookings per participant per UTC day of StartUtc, across all rooms.
        var sameDayActive = existingPersonBookings.Count(b =>
            b.IsActive &&
            b.BookedBy == bookedBy &&
            b.Range.StartUtc.Date == range.StartUtc.Date);

        if (sameDayActive >= MaxActivePerParticipantPerDay)
            return BookingErrors.DailyLimitReached;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy,
            Range = range,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    // R8: Active -> Cancelled; cancelling twice is a conflict.
    public Result Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Result.Success();
    }
}
