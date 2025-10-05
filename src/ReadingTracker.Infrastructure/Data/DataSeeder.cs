using System.Text.Json;
using Microsoft.Extensions.Logging;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Infrastructure.Data;

public class DataSeeder
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(IBookRepository bookRepository, ILogger<DataSeeder> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task SeedBooksAsync()
    {
        try
        {
            var existingBooks = await _bookRepository.GetAllAsync();
            if (existingBooks.Any())
            {
                _logger.LogInformation("Books already exist in the repository, skipping seed data loading");
                return;
            }

            var seedFilePath = Path.Combine(AppContext.BaseDirectory, "data", "books.seed.json");

            if (!File.Exists(seedFilePath))
            {
                _logger.LogWarning("Seed data file not found at {Path}, starting with empty catalog", seedFilePath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(seedFilePath);
            
            var seedData = JsonSerializer.Deserialize<BookSeedData[]>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (seedData == null || !seedData.Any())
            {
                _logger.LogWarning("No seed data found in {Path}, starting with empty catalog", seedFilePath);
                return;
            }

            _logger.LogInformation("Loading {Count} books from seed data", seedData.Length);

            var books = seedData.Select(seedBook => new Book
            {
                Id = Guid.NewGuid(),
                Title = seedBook.Title,
                Author = seedBook.Author,
                Year = seedBook.Year,
                Pages = seedBook.Pages,
                Genres = seedBook.Genres,
                Rating = null
            }).ToList();

            await _bookRepository.AddRangeAsync(books);

            _logger.LogInformation("Successfully loaded {Count} books from seed data", seedData.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading seed data");
        }
    }
}
