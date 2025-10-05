using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Infrastructure.Data;
using ReadingTracker.Infrastructure.Repositories;

namespace ReadingTracker.Tests.UnitTests.Infrastructure.Repositories;

public class BookRepositoryTests : IDisposable
{
    private readonly ReadingTrackerDbContext _context;
    private readonly BookRepository _repository;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ReadingTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ReadingTrackerDbContext(options);
        _repository = new BookRepository(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Book_To_Database()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300,
            Genres = new[] { "Fiction" }
        };

        // Act
        var result = await _repository.AddAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(book.Id);

        var savedBook = await _context.Books.FindAsync(book.Id);
        savedBook.Should().NotBeNull();
        savedBook!.Title.Should().Be("Test Book");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Book_When_Found()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300
        };

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(book.Id);
        result.Title.Should().Be("Test Book");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Book_In_Database()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            Author = "Test Author",
            Year = 2023,
            Pages = 300,
            Rating = null
        };

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        book.Title = "Updated Title";
        book.Rating = 5;
        var result = await _repository.UpdateAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Rating.Should().Be(5);

        var updatedBook = await _context.Books.FindAsync(book.Id);
        updatedBook!.Title.Should().Be("Updated Title");
        updatedBook.Rating.Should().Be(5);
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Book_Exists()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300
        };

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(book.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_False_When_Book_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
