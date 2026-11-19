namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public sealed record Request(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

    public sealed record Response(Guid Id, Guid RoomId, string Status);

    // Mapped on the "/api/rooms/{roomId}/bookings" group.
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", (Guid roomId, Request request, CreateBookingHandler handler) =>
        {
            var booking = handler.Handle(
                roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);

            return TypedResults.Created(
                $"/api/bookings/{booking.Id}",
                new Response(booking.Id, booking.RoomId, booking.Status.ToString()));
        });
    }
}
