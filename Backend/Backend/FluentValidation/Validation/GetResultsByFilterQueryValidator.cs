using Backend.MediatR.Queries;
using Backend.Services;
using FluentValidation;

namespace Backend.FluentValidation.Validation;

public class GetResultsByFilterQueryValidator : AbstractValidator<GetResultsByFilterQuery>
{
    public GetResultsByFilterQueryValidator()
    {
        RuleFor(x => x.Filter.FileName)
                .NotEmpty()
                .When(x => x.Filter.FileName != null)
                .WithMessage("FileName не может быть пустым, если указан.");

        RuleFor(x => x)
                .Must(x => !x.Filter.StartDateFrom.HasValue || !x.Filter.StartDateTo.HasValue || x.Filter.StartDateFrom <= x.Filter.StartDateTo)
                .WithMessage("StartDateFrom не может быть позже StartDateTo.");

            RuleFor(x => x)
                .Must(x => !x.Filter.AvgValueFrom.HasValue || !x.Filter.AvgValueTo.HasValue || x.Filter.AvgValueFrom <= x.Filter.AvgValueTo)
                .WithMessage("AvgValueFrom не может быть больше AvgValueTo.");

            RuleFor(x => x)
                .Must(x => !x.Filter.AvgExecutionTimeFrom.HasValue || !x.Filter.AvgExecutionTimeTo.HasValue || x.Filter.AvgExecutionTimeFrom <= x.Filter.AvgExecutionTimeTo)
                .WithMessage("AvgExecutionTimeFrom не может быть больше AvgExecutionTimeTo.");

            RuleFor(x => x.Filter.AvgValueFrom)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.AvgValueFrom.HasValue)
                .WithMessage("AvgValueFrom не может быть отрицательным.");

            RuleFor(x => x.Filter.AvgValueTo)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.AvgValueTo.HasValue)
                .WithMessage("AvgValueTo не может быть отрицательным.");

            RuleFor(x => x.Filter.AvgExecutionTimeFrom)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.AvgExecutionTimeFrom.HasValue)
                .WithMessage("AvgExecutionTimeFrom не может быть отрицательным.");

            RuleFor(x => x.Filter.AvgExecutionTimeTo)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.AvgExecutionTimeTo.HasValue)
                .WithMessage("AvgExecutionTimeTo не может быть отрицательным.");
    }
}