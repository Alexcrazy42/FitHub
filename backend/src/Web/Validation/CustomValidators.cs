using FitHub.Common.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace FitHub.Web.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBe<T, TElement>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Action<TElement> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            try
            {
                factoryMethod(value);
            }
            catch (CommonException e)
            {
                context.AddFailure(e.Message);
            }
        });
    }

    public static async Task HandleValidationAsync<TRequest>(this IValidator<TRequest>? validator, TRequest request, CancellationToken ct)
        where TRequest : class
    {
        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            validationResult.HandleValidationResult();
        }
    }

    private static void HandleValidationResult(this ValidationResult result)
    {
        if (result.IsValid)
        {
            return;
        }

        var errors = result.Errors;
        var responseErrors = errors.Select(error => new ValidationError(error.ErrorMessage, error.PropertyName)).ToList();
        throw new DetailedValidationException(responseErrors);
    }
}
