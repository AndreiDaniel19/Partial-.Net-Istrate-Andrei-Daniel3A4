using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    // Read-only query: no command handler. At Level C this reads the DbContext with AsNoTracking().
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async Task<Results<Ok<Response>, ProblemHttpResult>> (
            Guid id,
            IRoomRepository rooms,
            CancellationToken cancellationToken) =>
        {
            var room = await rooms.GetByIdAsync(id, cancellationToken);
            if (room is null)
                return RoomErrors.NotFound.ToProblem();

            return TypedResults.Ok(new Response(room.Id, room.Name, room.Capacity));
        });
    }
}
