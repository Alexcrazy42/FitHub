using FitHub.Web.V1.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Di;

public class DependenciesTests : IClassFixture<ContainerFixture>
{
    private readonly IServiceProvider container;

    public DependenciesTests(ContainerFixture fixture)
    {
        container = fixture.Container;
    }

    [Theory(DisplayName = "Resolve controller")]
    [MemberData(nameof(Controllers))]
    internal void ControllerShouldBeResolved(Type controllerType)
    {
        var instance = container.GetRequiredService(controllerType);

        instance.ShouldNotBeNull();
    }

    /// <summary>
    /// All API controllers
    /// </summary>
    public static IEnumerable<object[]> Controllers =>
        typeof(ChatController).Assembly
            .DefinedTypes
            .Where(type => type.IsAssignableTo(typeof(ControllerBase)))
            .Where(type => !type.IsAbstract)
            .Select(type => new[] { type });
}
