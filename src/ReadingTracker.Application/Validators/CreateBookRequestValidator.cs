using FluentValidation;
using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Validators;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required");

        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage("Author is required");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(1900)
            .WithMessage("Year must be >= 1900");

        RuleFor(x => x.Pages)
            .GreaterThan(0)
            .WithMessage("Pages must be > 0");
    }
}
