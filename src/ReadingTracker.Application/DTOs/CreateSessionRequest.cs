namespace ReadingTracker.Application.DTOs;

public class CreateSessionRequest
{
    public Guid BookId { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
}
