using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TickerQ.Jobs.Utilities.Objects;

public static class ObjectExtensions
{
    public static T Required<T>([NotNull] this T? value, [CallerArgumentExpression("value")] string? paramName = null)
        where T : class
        => value ?? throw new ArgumentNullException(paramName);

    public static T Required<T>([NotNull] this T? value, [CallerArgumentExpression("value")] string? paramName = null)
        where T : struct
        => value ?? throw new ArgumentNullException(paramName);
}
