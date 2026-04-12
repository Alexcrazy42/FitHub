using FitHub.BankManager.Web.Contracts;
using FitHub.Common.Http;
using Microsoft.Extensions.Options;

namespace FitHub.BankManager.Clients.Payment;

public class BankManagerPaymentClient : IBankManagerPaymentClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public BankManagerPaymentClient(HttpClient client, IOptions<BankManagerClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }

    public async Task<BankManagerPaymentIntentResult> CreatePaymentIntentAsync(
        string externalReference,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken ct)
    {
        var response = await client.PostAsJsonAsync<CreatePaymentIntentRequest, PaymentIntentResponse>(
            new Uri(baseUri, "/api/v1/bank/payment-intents"),
            new CreatePaymentIntentRequest(externalReference, amount, currency, idempotencyKey),
            ct);

        return ToResult(response);
    }

    public async Task<BankManagerPaymentIntentResult> CompletePaymentIntentAsync(
        string paymentIntentId,
        bool succeeded,
        string externalEventId,
        string? failureReason,
        CancellationToken ct)
    {
        var response = await client.PostAsJsonAsync<CompletePaymentIntentRequest, PaymentIntentResponse>(
            new Uri(baseUri, $"/api/v1/bank/payment-intents/{paymentIntentId}/complete"),
            new CompletePaymentIntentRequest(succeeded, externalEventId, failureReason),
            ct);

        return ToResult(response);
    }

    public async Task<PublishOutboxMessagesResult> PublishOutboxAsync(CancellationToken ct)
    {
        var response = await client.PostAsync<PublishOutboxMessagesResponse>(
            new Uri(baseUri, "/api/v1/bank/jobs/outbox/publish"),
            ct);

        return response is null
            ? new PublishOutboxMessagesResult(0, 0)
            : new PublishOutboxMessagesResult(response.PublishedCount, response.FailedCount);
    }

    private static BankManagerPaymentIntentResult ToResult(PaymentIntentResponse? response)
    {
        if (response is null)
        {
            throw new InvalidOperationException("BankManager returned empty payment intent response.");
        }

        return new BankManagerPaymentIntentResult(
            response.Id,
            response.ExternalReference,
            response.Amount.Amount,
            response.Amount.Currency,
            response.Status,
            response.FailureReason);
    }
}
