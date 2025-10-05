using FluentAssertions;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Validators;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Tests.UnitTests.Application.Validators;

public class CreateBookRequestValidatorTests
{
    private readonly CreateBookRequestValidator _validator;

    public CreateBookRequestValidatorTests()
    {
        _validator = new CreateBookRequestValidator();
    }

    [Fact]
    public void Should_Pass_When_All_Fields_Valid()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Valid Title",
            Author = "Valid Author",
            Year = 2023,
            Pages = 300,
            Genres = new[] { "Fiction", "Adventure" }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Title_Missing()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Author = "Valid Author",
            Year = 2023,
            Pages = 300
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Should_Fail_When_Author_Missing()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Valid Title",
            Year = 2023,
            Pages = 300
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author");
    }

    [Fact]
    public void Should_Fail_When_Year_Zero()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Valid Title",
            Author = "Valid Author",
            Year = 0,
            Pages = 300
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }

    [Fact]
    public void Should_Fail_When_Year_Negative()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Valid Title",
            Author = "Valid Author",
            Year = -100,
            Pages = 300
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }

    [Fact]
    public void Should_Fail_When_Pages_Zero()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Valid Title",
            Author = "Valid Author",
            Year = 2023,
            Pages = 0
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Pages");
    }

    [Fact]
    public void Should_Fail_When_Pages_Negative()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Valid Title",
            Author = "Valid Author",
            Year = 2023,
            Pages = -50
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Pages");
    }
}
