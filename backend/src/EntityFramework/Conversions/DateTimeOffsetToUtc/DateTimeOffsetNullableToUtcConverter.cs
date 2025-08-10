using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitHub.Common.EntityFramework.Conversions.DateTimeOffsetToUtc;

/// <summary>
/// Конвертер Nullable DateTimeOffset в Nullable DateTimeOffset в UTC для хранения в БД
/// </summary>
internal sealed class DateTimeOffsetNullableToUtcConverter : ValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public DateTimeOffsetNullableToUtcConverter()
        : base(model => model == null ? model : model.Value.ToUniversalTime(), provider => provider)
    {
    }
}
