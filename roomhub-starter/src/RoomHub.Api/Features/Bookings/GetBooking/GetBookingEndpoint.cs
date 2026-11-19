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

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", (Guid id, GetBookingHandler handler) =>
        {
            var b = handler.Handle(id);
            return TypedResults.Ok(new Response(
                b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
        });
    }
}
