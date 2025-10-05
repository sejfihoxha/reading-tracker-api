using FluentValidation;
using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Validators;

public class UpdateRatingRequestValidator : AbstractValidator<UpdateRatingRequest>
{
    public UpdateRatingRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5 or null");
    }
}
