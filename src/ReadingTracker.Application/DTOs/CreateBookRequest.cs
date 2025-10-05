namespace ReadingTracker.Application.DTOs;

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Pages { get; set; }
    public string[]? Genres { get; set; }
}
