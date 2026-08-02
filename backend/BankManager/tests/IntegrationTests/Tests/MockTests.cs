using FitHub.BankManager.Clients.Tests;
using FitHub.BankManager.IntegrationTests.Infrastructure;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace FitHub.BankManager.IntegrationTests.Tests;

public class MockTests : ControllerTestsBase
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly ITestClient sut;

    public MockTests(ServerFixture serverFixture, ITestOutputHelper testOutputHelper) : base(serverFixture)
    {
        this.testOutputHelper = testOutputHelper;
        sut = serverFixture.TestClient;
    }

    [Fact(DisplayName = "Test")]
    public async Task Test_FirstCall_ShouldReturn()
    {
        // arrange
        var test = "test";
        MockTestService.Setup(x => x.Test())
            .ReturnsAsync(test);

        // act
        var result = await sut.Test();

        // assert
        result.ShouldBe(test);
    }

    // этот код опирается на то, что мы засетапили мок MockTestService
    // поэтому он иногда может проходить успешно
    // но иногда он может и падать, тк xUnit не гарантирует нам порядок запуска тестов
    // [Fact(DisplayName = "Test1")]
    // public async Task Test_SecondCall_ShouldReturnSameValueAsFirstTest()
    // {
    //     // arrange
    //     var test = "test";
    //
    //     // act
    //     var result = await sut.Test();
    //
    //     // assert
    //     result.ShouldBe(test);
    // }
}
