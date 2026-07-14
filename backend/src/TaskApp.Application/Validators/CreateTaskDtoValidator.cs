using FluentValidation;
using TaskApp.Application.DTOs;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validation rules for <see cref="CreateTaskDto"/>: title is required and bounded,
/// priority must be one of the known enum values.
/// </summary>
public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    /// <summary>Defines the validation rules for <see cref="CreateTaskDto"/>.</summary>
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must be 200 characters or fewer.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be Low, Medium, or High.");
    }
}
