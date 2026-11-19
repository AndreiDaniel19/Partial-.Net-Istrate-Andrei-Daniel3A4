namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", (Guid id, GetRoomHandler handler) =>
        {
            var room = handler.Handle(id);
            return TypedResults.Ok(new Response(room.Id, room.Name, room.Capacity));
        });
    }
}
