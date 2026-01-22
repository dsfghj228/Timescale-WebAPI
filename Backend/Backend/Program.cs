using System.Reflection;
using Back_Quiz.Exceptions;
using Backend.Data;
using Backend.Exceptions;
using Backend.FluentValidation;
using Backend.Interfaces;
using Backend.Mapper;
using Backend.Repositories;
using Backend.Services;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (_, _) => false;
    options.Map<CustomExceptions>(ex => new ProblemDetails
    {
        Type = ex.Type,
        Title = ex.Title,
        Status = (int)ex.StatusCode,
        Detail = ex.Message
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddBehavior(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));
});
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("v1/swagger.json", "Backend V1"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseProblemDetails();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();