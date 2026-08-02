namespace FitHub.BankManager.Application.Mocks;

public class MockTestService : IMockTestService
{
    public Task<string> Test() => throw new NotImplementedException();
}
