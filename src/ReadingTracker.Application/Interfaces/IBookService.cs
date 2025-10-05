using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Interfaces;

public interface IBookService
{
    Task<PaginatedResponse<BookResponse>> GetBooksAsync(string? query,string? author, int? yearMin,
        int? yearMax,
        string sort,
        string order);

    Task<BookResponse?> GetByIdAsync(Guid id);
    Task<BookResponse> CreateAsync(CreateBookRequest request);
    Task<BookResponse?> UpdateRatingAsync(Guid id, int? rating);
    Task<BookStatsResponse?> GetStatsAsync(Guid id);
}
