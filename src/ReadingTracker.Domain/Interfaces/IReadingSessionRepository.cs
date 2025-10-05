using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Domain.Interfaces;

public interface IReadingSessionRepository
{
    Task<ReadingSession?> GetByIdAsync(Guid id);
    Task<IEnumerable<ReadingSession>> GetAllAsync();
    Task<IEnumerable<ReadingSession>> GetByBookIdAsync(Guid bookId);
    Task<ReadingSession> AddAsync(ReadingSession session);
    Task<ReadingSession?> UpdateAsync(ReadingSession session);
}
