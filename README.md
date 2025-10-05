# Reading Tracker API

A REST API for managing a local book catalog and reading sessions.

## Quick Start

### Prerequisites
- .NET 9.0 SDK

### Run the Application

```bash
# Build and run
dotnet run --project src/ReadingTracker.Presentation

# Or with hot reload
cd src/ReadingTracker.Presentation
dotnet watch run
```

### Access API
- **Swagger UI**: `https://localhost:5002/swagger/index.html`
- **API Base**: `https://localhost:5002/api`

### Run Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

## API Endpoints

### Books
- `GET /api/books` - Get all books with filtering
- `GET /api/books/{id}` - Get specific book
- `POST /api/books` - Create new book
- `PATCH /api/books/{id}/rating` - Update rating (1-5)
- `GET /api/books/{id}/stats` - Get reading statistics

### Reading Sessions
- `GET /api/reading-sessions` - Get all sessions with filters
- `POST /api/reading-sessions` - Start new reading session
- `PATCH /api/reading-sessions/{id}/stop` - Stop session with time/pages

## Tech Stack

- **Framework**: .NET 9.0, ASP.NET Core
- **Database**: SQLite (in-memory)
- **Architecture**: Clean Architecture (4 layers)
- **Validation**: FluentValidation
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, FluentAssertions

## Features

- Book catalog with advanced filtering and sorting
- Reading session tracking with time and page counts
- Comprehensive input validation
- In-memory SQLite database (data persists during app runtime)
- Automatic seed data loading (20 sample books)
- RESTful API with Swagger documentation
