using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Trainings;
using FitHub.Web.Validation;
using FluentValidation;

namespace FitHub.Web.V1.Trainings.Validators;

public class CreateBaseGroupTrainingRequestValidator : AbstractValidator<CreateBaseGroupTrainingRequest>
{
    public CreateBaseGroupTrainingRequestValidator()
    {
        RuleFor(x => x.Name).MustBe(BaseGroupTraining.ValidateName);
        RuleFor(x => x.Complexity).MustBe(BaseGroupTraining.ValidateComplexity);
    }
}


