using FitHub.BankManager.Domain;
using FitHub.BankManager.Web.Contracts;

namespace FitHub.BankManager.Web.Controllers;

public static class PaymentIntentResponseExtensions
{
    public static PaymentIntentResponse ToResponse(this PaymentIntent intent)
    {
        return new PaymentIntentResponse(
            intent.Id.ToString(),
            intent.ExternalReference,
            new BankMoneyResponse(intent.Amount, intent.Currency),
            intent.Status.ToString(),
            intent.FailureReason,
            intent.CreatedAt,
            intent.UpdatedAt);
    }
}
