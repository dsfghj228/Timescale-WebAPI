using Back_Quiz.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomExceptions ex)
        {
            var problem = new ProblemDetails
            {
                Type = ex.Type,
                Title = ex.Title,
                Status = (int)ex.StatusCode,
                Detail = ex.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                ex.Message,
                problem
            });
        }
    }
}