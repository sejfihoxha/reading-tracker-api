using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Interfaces;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IReadingSessionRepository _sessionRepository;

    public BookService(IBookRepository bookRepository, IReadingSessionRepository sessionRepository)
    {
        _bookRepository = bookRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<PaginatedResponse<BookResponse>> GetBooksAsync(
        string? query,
        string? author,
        int? yearMin,
        int? yearMax,
        string sort,
        string order)
    {
        var books = await _bookRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            books = books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            books = books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
        }

        if (yearMin.HasValue)
        {
            books = books.Where(b => b.Year >= yearMin.Value);
        }

        if (yearMax.HasValue)
        {
            books = books.Where(b => b.Year <= yearMax.Value);
        }

        books = sort.ToLower() switch
        {
            "year" => order.ToLower() == "desc" 
                ? books.OrderByDescending(b => b.Year) 
                : books.OrderBy(b => b.Year),
            _ => order.ToLower() == "desc" 
                ? books.OrderByDescending(b => b.Title) 
                : books.OrderBy(b => b.Title)
        };

        var booksList = books.ToList();

        return new PaginatedResponse<BookResponse>
        {
            Items = booksList.Select(MapToResponse).ToList(),
            Total = booksList.Count
        };
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : MapToResponse(book);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            Year = request.Year,
            Pages = request.Pages,
            Genres = request.Genres ?? Array.Empty<string>(),
            Rating = null
        };

        var createdBook = await _bookRepository.AddAsync(book);
        return MapToResponse(createdBook);
    }

    public async Task<BookResponse?> UpdateRatingAsync(Guid id, int? rating)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return null;
        }

        book.Rating = rating;
        var updatedBook = await _bookRepository.UpdateAsync(book);
        return updatedBook == null ? null : MapToResponse(updatedBook);
    }

    public async Task<BookStatsResponse?> GetStatsAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return null;
        }

        var sessions = await _sessionRepository.GetByBookIdAsync(id);
        var completedSessions = sessions.Where(s => s.StoppedAt.HasValue).ToList();

        return new BookStatsResponse
        {
            BookId = id,
            SessionsCount = completedSessions.Count,
            TotalMinutes = completedSessions.Sum(s => s.Minutes ?? 0),
            TotalPagesRead = completedSessions.Sum(s => s.PagesRead ?? 0)
        };
    }

    private static BookResponse MapToResponse(Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year,
            Pages = book.Pages,
            Genres = book.Genres,
            Rating = book.Rating
        };
    }
}
