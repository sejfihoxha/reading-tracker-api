using FluentAssertions;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Validators;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Tests.UnitTests.Application.Validators;

public class CreateSessionRequestValidatorTests
{
    private readonly CreateSessionRequestValidator _validator;
    private readonly IBookRepository _bookRepository;

    public CreateSessionRequestValidatorTests()
    {
        // Mock repository for validator
        _bookRepository = Moq.Mock.Of<IBookRepository>();
        _validator = new CreateSessionRequestValidator(_bookRepository);
    }

    [Fact]
    public void Should_Pass_When_BookId_Is_Provided()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            BookId = Guid.NewGuid(),
            StartedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_BookId_Is_Empty()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            BookId = Guid.Empty,
            StartedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BookId");
    }

    [Fact]
    public void Should_Pass_When_StartedAt_Is_Null()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            BookId = Guid.NewGuid(),
            StartedAt = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_StartedAt_Is_In_Past()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            BookId = Guid.NewGuid(),
            StartedAt = DateTimeOffset.UtcNow.AddHours(-1)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_StartedAt_Is_In_Future()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            BookId = Guid.NewGuid(),
            StartedAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
