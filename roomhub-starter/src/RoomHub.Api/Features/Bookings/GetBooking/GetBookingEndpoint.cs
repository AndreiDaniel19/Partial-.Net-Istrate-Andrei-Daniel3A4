using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Infrastructure.Persistence;

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

    // Read-only query: reads the DbContext directly, without tracking.
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async Task<Results<Ok<Response>, ProblemHttpResult>> (
            Guid id,
            RoomHubDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var b = await dbContext.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (b is null)
                return BookingErrors.NotFound.ToProblem();

            return TypedResults.Ok(new Response(
                b.Id, b.RoomId, b.BookedBy, b.Range.StartUtc, b.Range.EndUtc, b.Attendees, b.Status.ToString()));
        });
    }
}
