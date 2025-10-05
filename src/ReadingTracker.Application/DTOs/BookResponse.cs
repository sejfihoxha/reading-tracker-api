namespace ReadingTracker.Application.DTOs;

public class BookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Pages { get; set; }
    public string[] Genres { get; set; } = Array.Empty<string>();
    public int? Rating { get; set; }
}
