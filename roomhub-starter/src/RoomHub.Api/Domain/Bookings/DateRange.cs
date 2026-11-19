using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record DateRange
{
    private DateRange(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; private init; }
    public DateTime EndUtc { get; private init; }

    public TimeSpan Duration => EndUtc - StartUtc;

    // R3: the end must be strictly after the start.
    public static Result<DateRange> Create(DateTime startUtc, DateTime endUtc)
    {
        startUtc = ToUtc(startUtc);
        endUtc = ToUtc(endUtc);

        if (endUtc <= startUtc)
            return BookingErrors.InvalidRange;

        return new DateRange(startUtc, endUtc);
    }

    // Values with an offset arrive as Local; values without one are treated as UTC.
    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    // Half-open interval [start, end): ranges that only touch do not overlap (R7).
    public bool Overlaps(DateRange other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;
}
