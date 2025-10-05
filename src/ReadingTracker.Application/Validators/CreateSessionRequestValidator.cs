using FluentValidation;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Application.Validators;

public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
{
    public CreateSessionRequestValidator(IBookRepository bookRepository)
    {
        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("BookId is required");
    }
}
