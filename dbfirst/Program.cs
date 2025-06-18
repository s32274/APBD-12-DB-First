using db_first.Models;
using Microsoft.EntityFrameworkCore;
using Tutorial9.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Rejestracja zaleznosci
builder.Services.AddDbContext<APBDContext>(opt => 
{
    // var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    // opt.UseSqlServer(connectionString);
    
    // nie dziala to
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();