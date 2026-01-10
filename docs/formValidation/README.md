# FormValidation

Динамическая связка правил форм: FluentValidation + автоматический маппинг в ProblemDetails через Error Handler. На фронтенде React Hook Form с автоматическим парсингом ответа

Пример:
Форма:
![form](../../images/fv_form.png)

Поле дата открытия зависит от полей "Состояние" и "Активен" и должно быть заполнено, если транажер неактивен или не находится в состоянии "Готов к использованию".

Кейс 1 валидации:
![case 1](../../images/form_validation_case1.png)
Кейс 2 валидации:
![case 2](../../images/form_validation_case2.png)
Ответ от бэкенда:
![problem details response](../../images/problem_details_response.png)


## Решение
Все что нужно сделать на бэкенде это написать валидатор, привычный для FluentValidation. Единственное что, приходится писать static функции, и я их пишу прям в requests
```csharp
public class AddOrUpdateGymEquipmentValidator : AbstractValidator<AddOrUpdateGymEquipmentRequest>
{
    public AddOrUpdateGymEquipmentValidator()
    {
        RuleFor(x => x.EquipmentId).MustBe(AddOrUpdateGymEquipmentRequest.ValidateEquipment);
        RuleFor(x => x.GymId).MustBe(AddOrUpdateGymEquipmentRequest.ValidateGym);
        RuleFor(x => x.IsActive).MustBe(AddOrUpdateGymEquipmentRequest.ValidateIsActive);
        RuleFor(x => x.Condition).MustBe(AddOrUpdateGymEquipmentRequest.ValidateCondition);

        RuleFor(x => x).MustBeValid(AddOrUpdateGymEquipmentRequest.ValidateInactiveWithOpenedAt, nameof(AddOrUpdateGymEquipmentRequest.IsActive));
        RuleFor(x => x).MustBeValid(AddOrUpdateGymEquipmentRequest.ValidateActiveWithOpenedAt, nameof(AddOrUpdateGymEquipmentRequest.OpenedAt));
        RuleFor(x => x).MustBeValid(AddOrUpdateGymEquipmentRequest.ValidateMaintenanceWithOpenedAt, nameof(AddOrUpdateGymEquipmentRequest.OpenedAt));
    }
}
```

Функция на фронтенде, которая автоматически маппит ответ бэкенда в RHF:
```typescript
/**
 * Маппит серверные ошибки валидации на поля формы react-hook-form
 * @param errors - массив ошибок с бэкенда
 * @param setError - функция setError из useForm
 */
export function mapServerValidationErrors<T extends FieldValues>(
  errors: ValidationError[] | undefined,
  setError: UseFormSetError<T>
): void {
  if (!errors || errors.length === 0) return;

  const mapPropertyToField = (propertyName: string): string => {
    if (!propertyName) return propertyName;
    return propertyName.charAt(0).toLowerCase() + propertyName.slice(1);
  };

  for (const err of errors) {
    const field = mapPropertyToField(err.propertyName);
    if (!field) continue;
    
    setError(field as unknown, { 
      type: "server", 
      message: err.message 
    });
  }
}
```


Extensions методы для IRuleBuilder:
```csharp
public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBe<T, TElement>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Action<TElement> validationMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            try
            {
                validationMethod(value);
            }
            catch (CommonException e)
            {
                context.AddFailure(e.Message);
            }
        });
    }

    public static IRuleBuilderOptionsConditions<T, T> MustBeValid<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        Action<T> validationMethod,
        string propertyName)
    {
        return ruleBuilder.Custom((model, context) =>
        {
            try
            {
                validationMethod(model);
            }
            catch (CommonException e)
            {
                context.AddFailure(propertyName, e.Message);
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
```