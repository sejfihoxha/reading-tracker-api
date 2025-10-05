using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Interfaces;

public interface IReadingSessionService
{
    Task<PaginatedResponse<ReadingSessionResponse>> GetSessionsAsync(Guid? bookId, DateTimeOffset? from, DateTimeOffset? to);
    Task<ReadingSessionResponse> CreateAsync(CreateSessionRequest request);
    Task<ReadingSessionResponse?> StopAsync(Guid id, StopSessionRequest request);
}
