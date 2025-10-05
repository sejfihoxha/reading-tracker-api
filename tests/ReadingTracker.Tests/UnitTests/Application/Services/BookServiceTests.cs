using FluentAssertions;
using Moq;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Services;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Tests.UnitTests.Application.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IReadingSessionRepository> _sessionRepositoryMock;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _sessionRepositoryMock = new Mock<IReadingSessionRepository>();
        _service = new BookService(_bookRepositoryMock.Object, _sessionRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Book_With_Generated_Id()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300,
            Genres = new[] { "Fiction", "Adventure" }
        };

        var createdBook = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            Year = request.Year,
            Pages = request.Pages,
            Genres = request.Genres,
            Rating = null
        };

        _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>())).ReturnsAsync(createdBook);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be(request.Title);
        result.Author.Should().Be(request.Author);
        result.Year.Should().Be(request.Year);
        result.Pages.Should().Be(request.Pages);
        result.Genres.Should().BeEquivalentTo(request.Genres);
        result.Rating.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Book_When_Found()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300
        };

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);

        // Act
        var result = await _service.GetByIdAsync(bookId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(bookId);
        result.Title.Should().Be("Test Book");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.GetByIdAsync(bookId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRatingAsync_Should_Return_Null_When_Book_Not_Found()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.UpdateRatingAsync(bookId, 4);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRatingAsync_Should_Update_Rating_When_Book_Found()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300,
            Rating = null
        };

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>())).ReturnsAsync((Book b) => b);

        // Act
        var result = await _service.UpdateRatingAsync(bookId, 4);

        // Assert
        result.Should().NotBeNull();
        result!.Rating.Should().Be(4);
        _bookRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.Rating == 4)), Times.Once);
    }

    [Fact]
    public async Task UpdateRatingAsync_Should_Clear_Rating_When_Null_Provided()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300,
            Rating = 5
        };

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>())).ReturnsAsync((Book b) => b);

        // Act
        var result = await _service.UpdateRatingAsync(bookId, null);

        // Assert
        result.Should().NotBeNull();
        result!.Rating.Should().BeNull();
        _bookRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.Rating == null)), Times.Once);
    }

    [Fact]
    public async Task GetStatsAsync_Should_Return_Null_When_Book_Not_Found()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.GetStatsAsync(bookId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStatsAsync_Should_Calculate_Stats_Accurately_For_Single_Book()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300
        };

        var sessions = new List<ReadingSession>
        {
            new ReadingSession
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                StartedAt = DateTimeOffset.UtcNow.AddHours(-3),
                StoppedAt = DateTimeOffset.UtcNow.AddHours(-2),
                Minutes = 60,
                PagesRead = 50
            },
            new ReadingSession
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                StartedAt = DateTimeOffset.UtcNow.AddHours(-1),
                StoppedAt = DateTimeOffset.UtcNow,
                Minutes = 45,
                PagesRead = 30
            }
        };

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _sessionRepositoryMock.Setup(r => r.GetByBookIdAsync(bookId)).ReturnsAsync(sessions);

        // Act
        var result = await _service.GetStatsAsync(bookId);

        // Assert
        result.Should().NotBeNull();
        result!.TotalMinutes.Should().Be(105); // 60 + 45
        result.TotalPagesRead.Should().Be(80); // 50 + 30
        result.SessionsCount.Should().Be(2);
    }

    [Fact]
    public async Task GetStatsAsync_Should_Return_Zero_Stats_For_Book_With_No_Sessions()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Year = 2023,
            Pages = 300
        };

        var sessions = new List<ReadingSession>(); // Empty list

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _sessionRepositoryMock.Setup(r => r.GetByBookIdAsync(bookId)).ReturnsAsync(sessions);

        // Act
        var result = await _service.GetStatsAsync(bookId);

        // Assert
        result.Should().NotBeNull();
        result!.TotalMinutes.Should().Be(0);
        result.TotalPagesRead.Should().Be(0);
        result.SessionsCount.Should().Be(0);
    }
}
