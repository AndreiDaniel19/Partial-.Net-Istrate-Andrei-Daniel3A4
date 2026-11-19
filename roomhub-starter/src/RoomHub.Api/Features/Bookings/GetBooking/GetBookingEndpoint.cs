using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public sealed record Response(
        Guid Id,
        Guid RoomId,
        string BookedBy,
        DateTime StartUtc,
        DateTime EndUtc,
        int Attendees,
        string Status);

    // Read-only query: no command handler. At Level C this reads the DbContext with AsNoTracking().
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async Task<Results<Ok<Response>, ProblemHttpResult>> (
            Guid id,
            IBookingRepository bookings,
            CancellationToken cancellationToken) =>
        {
            var b = await bookings.GetByIdAsync(id, cancellationToken);
            if (b is null)
                return BookingErrors.NotFound.ToProblem();

            return TypedResults.Ok(new Response(
                b.Id, b.RoomId, b.BookedBy, b.Range.StartUtc, b.Range.EndUtc, b.Attendees, b.Status.ToString()));
        });
    }
}
