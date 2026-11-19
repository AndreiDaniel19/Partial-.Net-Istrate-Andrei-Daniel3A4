# BUGS

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| R7 (touching intervals) | `Services/BookingService.cs`, ~line 28 (`<=` instead of `<`) | Reading the code (10–12 followed by 12–14 threw 409) | Moved to `DateRange.Overlaps` using strict `<` |
| R7 / R8 (cancelled booking still blocks interval) | `Services/BookingService.cs`, ~line 26 (`ListForRoom` checks all bookings) | Reading the code | Filtered overlap check to only `Status == Active` |
| R8 (double cancel returns 204 instead of 409) | `Services/BookingService.cs`, ~line 59 (`Cancel` overwrites status without checking) | Reading the code | `Booking.Cancel()` returns Conflict (409) if not `Active` |
| R5 (booking in the past allowed) | `Services/BookingService.cs`, `Create` (~line 8): missing `now` check | Reading code + AI | Added `startUtc < nowUtc` validation (400) using injected `TimeProvider` |
| R6 (max 4h duration missing) | `Services/BookingService.cs`, `Create` (~line 17): only checks `endUtc <= startUtc` | Reading code + AI | Added check in `Booking.Create` to return 400 if duration exceeds 4h |