using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    // Read-only query: reads the DbContext directly, without tracking.
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async Task<Results<Ok<Response>, ProblemHttpResult>> (
            Guid id,
            RoomHubDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var response = await dbContext.Rooms
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new Response(r.Id, r.Name, r.Capacity))
                .FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                return RoomErrors.NotFound.ToProblem();

            return TypedResults.Ok(response);
        });
    }
}
