namespace ReadingTracker.Application.DTOs;

public class BookStatsResponse
{
    public Guid BookId { get; set; }
    public int SessionsCount { get; set; }
    public int TotalMinutes { get; set; }
    public int TotalPagesRead { get; set; }
}
