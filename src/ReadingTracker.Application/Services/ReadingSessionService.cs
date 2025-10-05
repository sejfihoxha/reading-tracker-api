using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Interfaces;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Application.Services;

public class ReadingSessionService : IReadingSessionService
{
    private readonly IReadingSessionRepository _sessionRepository;
    private readonly IBookRepository _bookRepository;

    public ReadingSessionService(IReadingSessionRepository sessionRepository, IBookRepository bookRepository)
    {
        _sessionRepository = sessionRepository;
        _bookRepository = bookRepository;
    }

    public async Task<PaginatedResponse<ReadingSessionResponse>> GetSessionsAsync(
        Guid? bookId,
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        var sessions = await _sessionRepository.GetAllAsync();

        if (bookId.HasValue)
        {
            sessions = sessions.Where(s => s.BookId == bookId.Value);
        }

        if (from.HasValue)
        {
            sessions = sessions.Where(s => s.StartedAt >= from.Value);
        }

        if (to.HasValue)
        {
            sessions = sessions.Where(s => s.StartedAt <= to.Value);
        }

        var sessionsList = sessions.ToList();

        return new PaginatedResponse<ReadingSessionResponse>
        {
            Items = sessionsList.Select(MapToResponse).ToList(),
            Total = sessionsList.Count
        };
    }

    public async Task<ReadingSessionResponse> CreateAsync(CreateSessionRequest request)
    {
        var bookExists = await _bookRepository.ExistsAsync(request.BookId);
        if (!bookExists)
        {
            throw new InvalidOperationException("Book not found");
        }

        var session = new ReadingSession
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            StartedAt = request.StartedAt ?? DateTimeOffset.UtcNow,
            StoppedAt = null,
            Minutes = null,
            PagesRead = null
        };

        var createdSession = await _sessionRepository.AddAsync(session);
        return MapToResponse(createdSession);
    }

    public async Task<ReadingSessionResponse?> StopAsync(Guid id, StopSessionRequest request)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
        {
            return null;
        }

        var stoppedAt = request.StoppedAt ?? DateTimeOffset.UtcNow;

        if (stoppedAt < session.StartedAt)
        {
            throw new InvalidOperationException("StoppedAt must be greater than or equal to StartedAt");
        }

        session.StoppedAt = stoppedAt;
        session.Minutes = request.Minutes;
        session.PagesRead = request.PagesRead;

        var updatedSession = await _sessionRepository.UpdateAsync(session);
        return updatedSession == null ? null : MapToResponse(updatedSession);
    }

    private static ReadingSessionResponse MapToResponse(ReadingSession session)
    {
        return new ReadingSessionResponse
        {
            Id = session.Id,
            BookId = session.BookId,
            StartedAt = session.StartedAt,
            StoppedAt = session.StoppedAt,
            Minutes = session.Minutes,
            PagesRead = session.PagesRead
        };
    }
}
