using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Domain;
using FitHub.BankManager.Web.Contracts;
using FitHub.Common.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.BankManager.Web.Controllers;

[ApiController]
[Route("api/v1/bank/payment-intents")]
public class PaymentIntentsController : ControllerBase
{
    private readonly IPaymentIntentService paymentIntentService;

    public PaymentIntentsController(IPaymentIntentService paymentIntentService)
    {
        this.paymentIntentService = paymentIntentService;
    }

    [HttpPost]
    public async Task<PaymentIntentResponse> CreateAsync([FromBody] CreatePaymentIntentRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");
        var intent = await paymentIntentService.CreateAsync(new CreatePaymentIntentCommand(
            request.ExternalReference,
            request.Amount,
            request.Currency,
            request.IdempotencyKey), ct);

        return intent.ToResponse();
    }

    [HttpGet("{id}")]
    public async Task<PaymentIntentResponse> GetAsync([FromRoute] string? id, CancellationToken ct)
    {
        var intent = await paymentIntentService.GetAsync(PaymentIntentId.Parse(id), ct);

        if (intent is null)
        {
            throw new NotFoundException("PaymentIntent не найден.");
        }

        return intent.ToResponse();
    }

    [HttpPost("{id}/complete")]
    public async Task<PaymentIntentResponse> CompleteAsync(
        [FromRoute] string? id,
        [FromBody] CompletePaymentIntentRequest? request,
        CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");
        var intent = await paymentIntentService.CompleteAsync(new CompletePaymentIntentCommand(
            PaymentIntentId.Parse(id),
            request.Succeeded,
            request.ExternalEventId,
            request.FailureReason), ct);

        return intent.ToResponse();
    }
}
