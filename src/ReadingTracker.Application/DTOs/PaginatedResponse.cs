namespace ReadingTracker.Application.DTOs;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    public int Total { get; set; }
}
