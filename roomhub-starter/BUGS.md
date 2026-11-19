# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| R7 (touching intervals) | `Services/BookingService.cs`, ~line 28: `existing.StartUtc <= endUtc && startUtc <= existing.EndUtc` | Reading the code. A booking 10:00–12:00 followed by 12:00–14:00 returns 409 | The overlap check moved to `DateRange.Overlaps` and uses strict `<` (half-open interval): `StartUtc < other.EndUtc && other.StartUtc < EndUtc` |
| R7 / R8 (a cancelled booking frees its interval) | `Services/BookingService.cs`, ~line 26: `foreach (var existing in bookings.ListForRoom(roomId))` checks all bookings, also the cancelled ones | Reading the code. Cancel a booking, then book the same interval again: still 409 | Only bookings with `Status == Active` are considered when checking for overlap |
| R8 (cancel an already cancelled booking gives 409) | `Services/BookingService.cs`, ~line 59–63, `Cancel`: sets `Status = Cancelled` without checking the current status, so a second cancel returns 204 | Reading the code | `Booking.Cancel()` in the domain returns `BookingErrors.AlreadyCancelled` (Conflict → 409) if the booking is not `Active` |
| R5 (a booking cannot start in the past, "now" from `TimeProvider`) | `Services/BookingService.cs`, `Create` (~lines 8–47): there is no check at all on `startUtc` vs. now, and `TimeProvider` is not used anywhere | Reading the code, with AI (Kiro). A booking in 2020 is accepted with 201 | `Booking.Create(..., DateTime nowUtc)` returns a Validation error (400) if `startUtc < nowUtc`; `nowUtc` comes from `TimeProvider.GetUtcNow()` injected in the handler |
| R6 (maximum duration 4 hours, exactly 4 allowed) | `Services/BookingService.cs`, `Create` (~lines 17–18): only `endUtc <= startUtc` is checked, there is no maximum duration | Reading the code, with AI (Kiro). A booking of 10 hours is accepted with 201 | `Booking.Create` returns a Validation error (400) if `endUtc - startUtc > 4 hours` (`TimeSpan.FromHours(4)` is still allowed) |
