namespace RoomHub.Api.Common.Cqrs;

// "No payload" result for commands such as CancelBooking.
public readonly record struct Unit
{
    public static readonly Unit Value = new();
}
