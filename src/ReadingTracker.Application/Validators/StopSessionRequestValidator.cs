using FluentValidation;
using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Validators;

public class StopSessionRequestValidator : AbstractValidator<StopSessionRequest>
{
    public StopSessionRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Minutes.HasValue || x.PagesRead.HasValue)
            .WithMessage("At least one of Minutes or PagesRead must be provided");

        RuleFor(x => x.Minutes)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Minutes.HasValue)
            .WithMessage("Minutes must be >= 1");

        RuleFor(x => x.PagesRead)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PagesRead.HasValue)
            .WithMessage("PagesRead must be >= 0");
    }
}
