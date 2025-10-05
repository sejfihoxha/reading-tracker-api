using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ReadingTracker.Application.Interfaces;
using ReadingTracker.Application.Services;
using ReadingTracker.Application.Validators;
using ReadingTracker.Domain.Interfaces;
using ReadingTracker.Infrastructure.Data;
using ReadingTracker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var sqliteConnection = new SqliteConnection("DataSource=InMemoryDatabase;Mode=Memory;Cache=Shared");
sqliteConnection.Open();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Reading Tracker API",
        Version = "v1",
        Description = "A REST API for managing book catalog and reading sessions"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<ReadingTrackerDbContext>(options =>
    options.UseSqlite("DataSource=InMemoryDatabase;Mode=Memory;Cache=Shared"));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReadingSessionRepository, ReadingSessionRepository>();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReadingSessionService, ReadingSessionService>();

builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReadingTrackerDbContext>();

    await context.Database.EnsureCreatedAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedBooksAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reading Tracker API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

sqliteConnection.Close();