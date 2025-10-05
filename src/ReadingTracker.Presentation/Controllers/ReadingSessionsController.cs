using ReadingTracker.Presentation.Extensions;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Interfaces;
using ReadingTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ReadingTracker.Presentation.Controllers;

/// <summary>
/// Manages reading session operations
/// </summary>
[Route("api/reading-sessions")]
[ApiController]
public class ReadingSessionsController : ControllerBase
{
    private readonly IReadingSessionService _sessionService;

    public ReadingSessionsController(IReadingSessionService sessionService)
    {
        _sessionService = sessionService;
    }


    /// <summary>
    /// Get reading sessions with optional filters
    /// </summary>
    /// <param name="bookId">Filter by book Id</param>
    /// <param name="from">Filter sessions where StartedAt is greater than or equal to from</param>
    /// <param name="to">Filter sessions where StartedAt is less than or equal to to</param>
    /// <returns>Paginated list of reading sessions</returns>
    /// <response code="200">Returns paginated list of reading sessions</response>
    [HttpGet]
    public async Task<IActionResult> GetSessions(
        [FromQuery] Guid? bookId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        var result = await _sessionService.GetSessionsAsync(bookId, from, to);
        return Ok(result);
    }

    /// <summary>
    /// Create a new reading session
    /// </summary>
    /// <param name="request">Session creation request with bookId and optional startedAt (defaults to current time if not provided)</param>
    /// <returns>Created reading session</returns>
    /// <response code="201">Returns the created reading session</response>
    /// <response code="400">Invalid request body or validation errors</response>
    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }

        var session = await _sessionService.CreateAsync(request);
        
        return CreatedAtAction(nameof(GetSessions), null, session);
    }

    /// <summary>
    /// Stop a reading session
    /// </summary>
    /// <param name="id">Session Id</param>
    /// <param name="request">Stop session request (stoppedAt defaults to current time if not provided)</param>
    /// <returns>Updated reading session</returns>
    /// <response code="200">Returns the updated reading session</response>
    /// <response code="400">Invalid request body or validation errors</response>
    /// <response code="404">Reading session with specified id not found</response>
    [HttpPatch("{id}/stop")]
    public async Task<IActionResult> StopSession([FromRoute] Guid id, [FromBody] StopSessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }

        var session = await _sessionService.StopAsync(id, request);
        
        if (session == null)
        {
            return NotFound($"Reading session with id {id} not found");
        }
        else
        {
            return Ok(session);
        }
    }
}
