using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

var connectionString = builder.Configuration.GetConnectionString("RoomHub") ?? "Data Source=roomhub.db";
builder.Services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app);

app.MapGet("/", () => "RoomHub API is running.");
app.MapRoomsEndpoints();
app.MapBookingsEndpoints();

app.Run();
