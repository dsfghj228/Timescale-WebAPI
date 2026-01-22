using Backend.MediatR.Commands;
using Backend.Services;
using FluentValidation;

namespace Backend.FluentValidation.Validation;

public class SaveFileCommandValidator : AbstractValidator<SaveFileCommand>
{
    public SaveFileCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .Must(FileValidationRules.CanRead)
            .WithMessage("File stream должен быть доступен для чтения.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(FileValidationRules.IsValidFileName)
            .WithMessage("Неверное file name.")
            .Must(FileValidationRules.HasCsvExtension)
            .WithMessage("Только .csv файлы поддерживаются.");
    }
}