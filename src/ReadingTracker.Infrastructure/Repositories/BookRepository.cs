using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;
using ReadingTracker.Infrastructure.Data;

namespace ReadingTracker.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ReadingTrackerDbContext _context;

        public BookRepository(ReadingTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task AddRangeAsync(IEnumerable<Book> books)
        {
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
        }

        public async Task<Book?> UpdateAsync(Book book)
        {
            var existing = await _context.Books.FindAsync(book.Id);
            if (existing == null)
            {
                return null;
            }

            _context.Entry(existing).CurrentValues.SetValues(book);
            // Manually update Genres array since SetValues doesn't handle it properly
            existing.Genres = book.Genres;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }
    }
}