using ReadingTracker.Presentation.Extensions;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Interfaces;
using ReadingTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ReadingTracker.Presentation.Controllers;

/// <summary>
/// Manages book catalog operations
/// </summary>
[Route("api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Get all books with optional filtering and sorting
    /// </summary>
    /// <param name="query">Filter by title (case-insensitive contains)</param>
    /// <param name="author">Filter by author (case-insensitive contains)</param>
    /// <param name="yearMin">Minimum year (inclusive)</param>
    /// <param name="yearMax">Maximum year (inclusive)</param>
    /// <param name="sort">Sort by 'title' or 'year' (default: 'title')</param>
    /// <param name="order">Sort order 'asc' or 'desc' (default: 'asc')</param>
    /// <returns>Paginated list of books</returns>
    [HttpGet]
    public async Task<IActionResult> GetBooks(
        [FromQuery] string? query,
        [FromQuery] string? author,
        [FromQuery] int? yearMin,
        [FromQuery] int? yearMax,
        [FromQuery] string sort = "title",
        [FromQuery] string order = "asc")
    {
        var result = await _bookService.GetBooksAsync(query, author, yearMin, yearMax, sort, order);
        
        return Ok(result);
    }

    /// <summary>
    /// Get a specific book by Id
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <returns>Book details</returns>
    /// <response code="200">Returns the book object</response>
    /// <response code="404">Book not found</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook([FromRoute] Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book == null)
        {
            return NotFound($"Book with id {id} not found");
        }

        return Ok(book);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    /// <param name="request">Book creation request with title, author, year, pages, and optional genres</param>
    /// <returns>Created book</returns>
    /// <response code="201">Returns the created book object</response>
    /// <response code="400">Validation error with details about invalid fields</response>
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }

        var book = await _bookService.CreateAsync(request);

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    /// <summary>
    /// Update or clear a book's rating
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <param name="request">Rating update request with rating (1-5) or null to clear</param>
    /// <returns>Updated book</returns>
    /// <response code="200">Returns the updated book object</response>
    /// <response code="400">Invalid rating value</response>
    /// <response code="404">Book not found</response>
    [HttpPatch("{id}/rating")]
    public async Task<IActionResult> UpdateRating([FromRoute] Guid id, [FromBody] UpdateRatingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.GetErrorMessages());
        }

        var book = await _bookService.UpdateRatingAsync(id, request.Rating);
        if (book == null)
        {
            return NotFound($"Book with id {id} not found");
        }

        return Ok(book);
    }

    /// <summary>
    /// Get reading statistics for a book
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <returns>Book statistics</returns>
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetBookStats([FromRoute] Guid id)
    {
        var stats = await _bookService.GetStatsAsync(id);

        if (stats == null)
        {
            return NotFound($"Book with id {id} not found");
        }

        return Ok(stats);
    }
}
