using AutoFixture;

namespace FitHub.Common.Autofixture;

public static class FixtureExtensions
{
    /// <summary>
    /// При создании инстанса <see cref="T"/> использовать конкретную фабрику
    /// </summary>
    public static void Customize<T>(this IFixture fixture, Func<T> factory)
        => fixture.Customize<T>(builder => builder.FromFactory(factory));
}
