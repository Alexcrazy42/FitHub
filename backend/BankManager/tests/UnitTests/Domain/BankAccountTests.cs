using FitHub.BankManager.Domain;
using FitHub.Common.Entities;
using Shouldly;
using Xunit;

namespace FitHub.BankManager.UnitTests.Domain;

public class BankAccountTests : DomainTestsBase
{
    [Fact]
    public void CreateBankAccount_JustCreated_NotDeactivated()
    {
        // arrange

        // act
        var bankAccount = CreateBankAccount();

        // assert
        bankAccount.IsActive.ShouldBe(true);
    }

    [Fact]
    public void DeactivateBankAccount_NotDeactivated_SuccessDeactivate()
    {
        // arrange
        var bankAccount = CreateBankAccount();

        // act
        bankAccount.Deactivate();

        // assert
        bankAccount.IsActive.ShouldBe(false);
    }

    [Fact]
    public void DeactivateBankAccount_Deactivated_ThrowException()
    {
        // arrange
        var bankAccount = CreateBankAccount();
        bankAccount.Deactivate();

        // act
        var act = () => bankAccount.Deactivate();

        // assert
        act.ShouldThrow<LogicViolationException>();
    }

    private BankAccount CreateBankAccount(string name = "", string currency = "")
    {
        return BankAccount.Create(name, currency);
    }
}
