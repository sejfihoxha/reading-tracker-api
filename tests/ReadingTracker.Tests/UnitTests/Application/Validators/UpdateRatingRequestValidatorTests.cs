using FluentAssertions;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Validators;

namespace ReadingTracker.Tests.UnitTests.Application.Validators;

public class UpdateRatingRequestValidatorTests
{
    private readonly UpdateRatingRequestValidator _validator;

    public UpdateRatingRequestValidatorTests()
    {
        _validator = new UpdateRatingRequestValidator();
    }

    [Fact]
    public void Should_Pass_When_Rating_Is_1()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = 1 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Rating_Is_5()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = 5 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Rating_Is_Null()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = null };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Rating_Is_3()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = 3 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Rating_Is_0()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = 0 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }

    [Fact]
    public void Should_Fail_When_Rating_Is_6()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = 6 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }

    [Fact]
    public void Should_Fail_When_Rating_Is_Negative()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = -1 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }

    [Fact]
    public void Should_Fail_When_Rating_Is_10()
    {
        // Arrange
        var request = new UpdateRatingRequest { Rating = 10 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }
}
