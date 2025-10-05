namespace ReadingTracker.Application.DTOs;

public class ReadingSessionResponse
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? StoppedAt { get; set; }
    public int? Minutes { get; set; }
    public int? PagesRead { get; set; }
}
