using FitHub.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitHub.Common.EntityFramework.Conversions.Identifiers;

/// <summary>
/// Конвертер нулябельных идентификаторов <see cref="GuidIdentifier{TId}"/> в нулябельную строку для хранения в БД
/// </summary>
internal sealed class GuidIdentifierNullableValueConverter<T> : ValueConverter<T?, Guid?>
    where T : IGuidIdentifier<T>
{
    public GuidIdentifierNullableValueConverter()
        : base(id => id == null ? null : id.Value, state => Parse(state))
    {
    }

    private static T? Parse(Guid? state) => T.TryParse(state, out var id) ? id : default;
}
