namespace RoomHub.Api.Common;

// Temporary (Level A): removed when the handlers return Result<T>.
public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException ex)
        {
            await Results.Problem(
                    detail: ex.Message,
                    statusCode: ex.StatusCode,
                    title: ex.Code)
                .ExecuteAsync(context);
        }
    }
}
