# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

I moved **R7 (no overlapping bookings)**. In the starter project, it lived in `Services/BookingService.cs` inside `Create` as a `foreach` loop using `<=` (which wrongly blocked touching intervals). Now it lives in `Domain/Bookings/DateRange.cs` (`Overlaps` using strict `<`) and in `Domain/Bookings/Booking.cs` (`Create`, filtering only active bookings). Putting it in the domain ensures a booking can never be created in an invalid state, regardless of which handler or endpoint triggers it.

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

A room already being occupied is a normal expected business outcome, not an unexpected crash, so using `Result<Booking>` avoids exception overhead. It makes failure explicit in the method signature. When an overlap happens in my code, `Booking.Create` returns `BookingErrors.Overlap` (tagged with `ErrorType.Conflict`). `CreateBookingHandler` passes that `Result` back to `CreateBookingEndpoint`, which calls `result.Error.ToProblem()` from `Common/ErrorHttpExtensions.cs` to return a `409 Conflict` `ProblemDetails` response without needing global exception middleware.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Only `Infrastructure/Persistence/` (`RoomHubDbContext`, entity configurations, `UtcDateTimeConverter`, repositories, `DatabaseInitializer`), `Program.cs`, and the read-only `GET` endpoints (using `AsNoTracking()`) know about EF Core. Everything in `Domain/` (`Booking`, `Room`, `DateRange`, repository interfaces, `IUnitOfWork`) and the command handlers are completely EF-free. With Repository + Unit of Work, I gained testable domain logic, easy persistence swapping (I switched from in-memory to SQLite without touching handlers), and single-transaction saves via `SaveChangesAsync`, at the cost of extra boilerplate interfaces and having to bypass repositories for efficient `GET` projections.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

```text
POST /api/rooms/{roomId}/bookings
  -> BookingsEndpoints -> CreateBookingEndpoint (builds CreateBookingCommand)
  -> ICommandHandler<CreateBookingCommand, Booking> (CreateBookingHandler)
  -> IRoomRepository.GetByIdAsync() (if null -> RoomErrors.NotFound -> 404)
  -> IBookingRepository.GetActiveForRoomAsync() & GetActiveForBookedByAsync()
  -> TimeProvider.GetUtcNow()
  -> Booking.Create() -> DateRange.Create() & DateRange.Overlaps() (checks R2-R10)
  -> IBookingRepository.Add() -> IUnitOfWork.SaveChangesAsync() (RoomHubDbContext)
  -> SQLite (roomhub.db)
  <- Result<Booking> -> TypedResults.Created (201) or result.Error.ToProblem() (400/404/409)
  ```

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

In roomhub.http, the AI initially hardcoded "bookedBy": "ana" across multiple test requests, and it also tried installing EF Core 11.0 which wasn't compatible with .NET 10. Because SQLite persists data between runs, the hardcoded "ana" failed on the second test run due to the daily active booking limit (R9). I rejected both—pinning EF Core to 10.0.15 and changing the test payload to "ana-{{roomId}}" so repeated test runs wouldn't collide.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

First, I would add unit tests for Booking.Create, Booking.Cancel, and DateRange.Overlaps using FakeTimeProvider, since right now I only rely on manual .http requests.
The biggest risk left in the project is concurrency: two simultaneous POST requests could both pass the R7/R9 overlap checks before either calls SaveChangesAsync, as there are no database-level constraints or serializable transactions protecting them.
