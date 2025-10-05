using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Infrastructure.Data;
using ReadingTracker.Infrastructure.Repositories;

namespace ReadingTracker.Tests.UnitTests.Infrastructure.Repositories;

public class ReadingSessionRepositoryTests : IDisposable
{
    private readonly ReadingTrackerDbContext _context;
    private readonly ReadingSessionRepository _repository;

    public ReadingSessionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ReadingTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ReadingTrackerDbContext(options);
        _repository = new ReadingSessionRepository(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Session_To_Database()
    {
        // Arrange
        var session = new ReadingSession
        {
            Id = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            StartedAt = DateTimeOffset.UtcNow.AddHours(-1),
            StoppedAt = DateTimeOffset.UtcNow,
            Minutes = 60,
            PagesRead = 50
        };

        // Act
        var result = await _repository.AddAsync(session);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(session.Id);

        var savedSession = await _context.ReadingSessions.FindAsync(session.Id);
        savedSession.Should().NotBeNull();
        savedSession!.Minutes.Should().Be(60);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Session_When_Found()
    {
        // Arrange
        var session = new ReadingSession
        {
            Id = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            StartedAt = DateTimeOffset.UtcNow.AddHours(-1),
            StoppedAt = null,
            Minutes = null,
            PagesRead = null
        };

        await _context.ReadingSessions.AddAsync(session);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(session.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(session.Id);
        result.StoppedAt.Should().BeNull();
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
    public async Task UpdateAsync_Should_Update_Session_In_Database()
    {
        // Arrange
        var session = new ReadingSession
        {
            Id = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            StartedAt = DateTimeOffset.UtcNow.AddHours(-2),
            StoppedAt = null,
            Minutes = null,
            PagesRead = null
        };

        await _context.ReadingSessions.AddAsync(session);
        await _context.SaveChangesAsync();

        // Act
        session.StoppedAt = DateTimeOffset.UtcNow;
        session.Minutes = 120;
        session.PagesRead = 80;
        var result = await _repository.UpdateAsync(session);

        // Assert
        result.Should().NotBeNull();
        result.StoppedAt.Should().NotBeNull();
        result.Minutes.Should().Be(120);
        result.PagesRead.Should().Be(80);

        var updatedSession = await _context.ReadingSessions.FindAsync(session.Id);
        updatedSession!.Minutes.Should().Be(120);
        updatedSession.PagesRead.Should().Be(80);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Sessions()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var sessions = new List<ReadingSession>
        {
            new ReadingSession
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                StartedAt = DateTimeOffset.UtcNow.AddHours(-3),
                StoppedAt = DateTimeOffset.UtcNow.AddHours(-2),
                Minutes = 60,
                PagesRead = 40
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

        await _context.ReadingSessions.AddRangeAsync(sessions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList.Should().Contain(s => s.Minutes == 60);
        resultList.Should().Contain(s => s.Minutes == 45);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Sessions()
    {
        // Arrange - No sessions added

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.ToList().Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
