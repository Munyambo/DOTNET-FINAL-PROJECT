using Microsoft.EntityFrameworkCore;
using RMSWebApi.Data;
using RMSWebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers ( our API Endpoints)
builder.Services.AddControllers();

// 3. OpenAPI description of the API (used by the Swagger page)
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "RMS API"));
}

app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

app.Run();
