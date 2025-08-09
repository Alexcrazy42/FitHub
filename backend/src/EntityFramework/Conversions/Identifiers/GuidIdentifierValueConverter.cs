using FitHub.Entities.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitHub.EntityFramework.Conversions.Identifiers;

/// <summary>
/// Конвертер идентификаторов <see cref="GuidIdentifier{TId}"/> в строку для хранения в БД
/// </summary>
internal sealed class GuidIdentifierValueConverter<T> : ValueConverter<T, Guid>
    where T : IGuidIdentifier<T>
{
    public GuidIdentifierValueConverter()
        : base(id => id.Value, state => Parse(state))
    {
    }

    private static T Parse(Guid state) => T.Parse(state);
}
