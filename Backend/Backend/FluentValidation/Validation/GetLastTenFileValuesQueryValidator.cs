using Backend.MediatR.Queries;
using FluentValidation;

namespace Backend.FluentValidation.Validation;

public class GetLastTenFileValuesQueryValidator : AbstractValidator<GetLastTenFileValuesQuery>
{
    public GetLastTenFileValuesQueryValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty();
    }
}