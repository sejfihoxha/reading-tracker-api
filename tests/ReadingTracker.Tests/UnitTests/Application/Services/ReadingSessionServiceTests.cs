using FluentAssertions;
using Moq;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Services;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Tests.UnitTests.Application.Services;

public class ReadingSessionServiceTests
{
    private readonly Mock<IReadingSessionRepository> _sessionRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly ReadingSessionService _service;

    public ReadingSessionServiceTests()
    {
        _sessionRepositoryMock = new Mock<IReadingSessionRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _service = new ReadingSessionService(_sessionRepositoryMock.Object, _bookRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Session_When_Book_Exists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow.AddHours(-1);
        var request = new CreateSessionRequest
        {
            BookId = bookId,
            StartedAt = startedAt
        };

        _bookRepositoryMock.Setup(r => r.ExistsAsync(bookId)).ReturnsAsync(true);
        _sessionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ReadingSession>()))
            .ReturnsAsync((ReadingSession s) => s);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.BookId.Should().Be(bookId);
        result.StartedAt.Should().Be(startedAt);
        result.StoppedAt.Should().BeNull();
        result.Minutes.Should().BeNull();
        result.PagesRead.Should().BeNull();

        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ReadingSession>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Default_StartedAt_To_Now_When_Not_Provided()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            BookId = bookId,
            StartedAt = null
        };

        var beforeCall = DateTimeOffset.UtcNow;

        _bookRepositoryMock.Setup(r => r.ExistsAsync(bookId)).ReturnsAsync(true);
        _sessionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ReadingSession>()))
            .ReturnsAsync((ReadingSession s) => s);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.StartedAt.Should().BeOnOrAfter(beforeCall);
        result.StartedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Book_Does_Not_Exist()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var request = new CreateSessionRequest { BookId = bookId };

        _bookRepositoryMock.Setup(r => r.ExistsAsync(bookId)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task StopAsync_Should_Return_Null_When_Session_Not_Found()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request = new StopSessionRequest();

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((ReadingSession?)null);

        // Act
        var result = await _service.StopAsync(sessionId, request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task StopAsync_Should_Stop_Session_With_Valid_Data()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow.AddHours(-2);
        var stoppedAt = DateTimeOffset.UtcNow;
        var existingSession = new ReadingSession
        {
            Id = sessionId,
            BookId = Guid.NewGuid(),
            StartedAt = startedAt,
            StoppedAt = null
        };

        var request = new StopSessionRequest
        {
            StoppedAt = stoppedAt,
            Minutes = 120,
            PagesRead = 50
        };

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(existingSession);
        _sessionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ReadingSession>()))
            .ReturnsAsync((ReadingSession s) => s);

        // Act
        var result = await _service.StopAsync(sessionId, request);

        // Assert
        result.Should().NotBeNull();
        result!.StoppedAt.Should().Be(stoppedAt);
        result.Minutes.Should().Be(120);
        result.PagesRead.Should().Be(50);

        _sessionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ReadingSession>()), Times.Once);
    }

    [Fact]
    public async Task StopAsync_Should_Default_StoppedAt_To_Now_When_Not_Provided()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow.AddHours(-1);
        var existingSession = new ReadingSession
        {
            Id = sessionId,
            BookId = Guid.NewGuid(),
            StartedAt = startedAt,
            StoppedAt = null
        };

        var request = new StopSessionRequest
        {
            StoppedAt = null,
            Minutes = 60,
            PagesRead = 30
        };

        var beforeCall = DateTimeOffset.UtcNow;

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(existingSession);
        _sessionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ReadingSession>()))
            .ReturnsAsync((ReadingSession s) => s);

        // Act
        var result = await _service.StopAsync(sessionId, request);

        // Assert
        result!.StoppedAt.Should().BeOnOrAfter(beforeCall);
        result.StoppedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task StopAsync_Should_Throw_When_Stop_Before_Start()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow;
        var stoppedAt = startedAt.AddHours(-1); // Before start time
        var existingSession = new ReadingSession
        {
            Id = sessionId,
            BookId = Guid.NewGuid(),
            StartedAt = startedAt,
            StoppedAt = null
        };

        var request = new StopSessionRequest
        {
            StoppedAt = stoppedAt,
            Minutes = 60,
            PagesRead = 30
        };

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(existingSession);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.StopAsync(sessionId, request));
    }
}
