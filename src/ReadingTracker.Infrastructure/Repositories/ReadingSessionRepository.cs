using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;
using ReadingTracker.Infrastructure.Data;

namespace ReadingTracker.Infrastructure.Repositories
{
    public class ReadingSessionRepository : IReadingSessionRepository
{
    private readonly ReadingTrackerDbContext _context;

    public ReadingSessionRepository(ReadingTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<ReadingSession?> GetByIdAsync(Guid id)
    {
        return await _context.ReadingSessions.FindAsync(id);
    }

    public async Task<IEnumerable<ReadingSession>> GetAllAsync()
    {
        return await _context.ReadingSessions.ToListAsync();
    }

    public async Task<IEnumerable<ReadingSession>> GetByBookIdAsync(Guid bookId)
    {
        return await _context.ReadingSessions
            .Where(s => s.BookId == bookId)
            .ToListAsync();
    }

    public async Task<ReadingSession> AddAsync(ReadingSession session)
    {
        _context.ReadingSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

        public async Task<ReadingSession?> UpdateAsync(ReadingSession session)
        {
            var existing = await _context.ReadingSessions.FindAsync(session.Id);
            if (existing == null)
            {
                return null;
            }

            _context.Entry(existing).CurrentValues.SetValues(session);
            await _context.SaveChangesAsync();
            return existing;
        }
}
}
