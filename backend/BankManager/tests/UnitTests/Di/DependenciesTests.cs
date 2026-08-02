using FitHub.BankManager.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace FitHub.BankManager.UnitTests.Di;

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

    public static IEnumerable<object[]> Controllers =>
        typeof(BankJobsController).Assembly
            .DefinedTypes
            .Where(type => type.IsAssignableTo(typeof(ControllerBase)))
            .Where(x => x.IsAbstract == false)
            .Select(type => new[] { type });
}
