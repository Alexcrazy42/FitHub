using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitHub.Common.EntityFramework.Conversions.DateTimeOffsetToUtc;

/// <summary>
/// Конвертер DateTimeOffset в DateTimeOffset в UTC для хранения в БД
/// </summary>
internal sealed class DateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetToUtcConverter()
        : base(model => model.ToUniversalTime(), provider => provider)
    {
    }
}
