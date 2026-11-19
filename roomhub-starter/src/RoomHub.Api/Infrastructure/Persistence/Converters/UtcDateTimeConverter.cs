using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RoomHub.Api.Infrastructure.Persistence.Converters;

// SQLite does not store DateTimeKind: values read back are marked as UTC again.
public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value,
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
