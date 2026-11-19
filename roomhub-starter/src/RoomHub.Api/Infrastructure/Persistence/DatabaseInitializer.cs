using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();

        if (dbContext.Database.GetMigrations().Any())
            await dbContext.Database.MigrateAsync();
        else
            await dbContext.Database.EnsureCreatedAsync();

        if (app.Environment.IsDevelopment())
            await SeedAsync(dbContext);
    }

    // Idempotent: only seeds an empty database.
    private static async Task SeedAsync(RoomHubDbContext dbContext)
    {
        if (await dbContext.Rooms.AnyAsync())
            return;

        var room = Room.Create("Study room 1", 10);
        if (room.IsSuccess)
        {
            dbContext.Rooms.Add(room.Value);
            await dbContext.SaveChangesAsync();
        }
    }
}
