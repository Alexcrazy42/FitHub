using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<ServerFixture>
{
    public const string Name = "Integration tests collection";

    public const string Category = "IntegrationTests";
}

