namespace ReadingTracker.Application.DTOs;

public class StopSessionRequest
{
    public DateTimeOffset? StoppedAt { get; set; }
    public int? Minutes { get; set; }
    public int? PagesRead { get; set; }
}
