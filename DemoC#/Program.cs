using DemoC_.Application;
using DemoC_.Infrastructure;
using DemoC_.Infrastructure.Data;
using DemoC_.Application.Interfaces;
using DemoC_.Middlewares;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


// Đăng ký toàn bộ Services của các Tầng (Clean Architecture DI)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);


// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Global Exception Handler Middleware
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();


