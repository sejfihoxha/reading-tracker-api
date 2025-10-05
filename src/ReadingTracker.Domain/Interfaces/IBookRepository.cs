using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book> AddAsync(Book book);
    Task AddRangeAsync(IEnumerable<Book> books);
    Task<Book?> UpdateAsync(Book book);
    Task<bool> ExistsAsync(Guid id);
}
