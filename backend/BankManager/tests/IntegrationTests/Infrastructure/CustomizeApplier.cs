using AutoFixture;
using FitHub.BankManager.Web.Contracts;

namespace FitHub.BankManager.IntegrationTests.Infrastructure;

internal static class ClassCustomizeApplier
{
    public static void ApplyCustomizes(IFixture fixture)
    {
        fixture.Customize(new CreatePaymentIntentRequestCustomization());
    }
}

internal class CreatePaymentIntentRequestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<CreatePaymentIntentRequest>(composer
            => composer.With(x => x.Currency, "USD")
        );
    }
}
