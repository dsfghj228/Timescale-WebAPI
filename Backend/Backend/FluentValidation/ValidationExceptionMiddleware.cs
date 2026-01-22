using FluentValidation;

namespace Backend.FluentValidation;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(x => new
            {
                x.PropertyName,
                x.ErrorMessage
            });

            await context.Response.WriteAsJsonAsync(new
            {
                context.Response.StatusCode,
                Message = "Validation failed",
                Errors = errors
            });
        }
    }
}