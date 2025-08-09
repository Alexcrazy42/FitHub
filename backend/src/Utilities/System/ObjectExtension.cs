using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FitHub.Utilities.System;

public static class ObjectExtension
{
    public static T Required<T>([NotNull] this T? value, [CallerArgumentExpression("value")] string? paramName = default)
        where T : class
        => value ?? throw new ArgumentNullException(paramName);

    public static T Required<T>([NotNull] this T? value, [CallerArgumentExpression("value")] string? paramName = default)
        where T : struct
        => value ?? throw new ArgumentNullException(paramName);
}
