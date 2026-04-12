using FitHub.Application.Outbox;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.Json;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Outbox;

namespace FitHub.Application.Marketplace;

public class MarketplacePaymentService : IMarketplacePaymentService
{
    private readonly IStockReservationRepository reservationRepository;
    private readonly IMarketplacePaymentRepository paymentRepository;
    private readonly IOutboxRepository outboxRepository;
    private readonly IUnitOfWork unitOfWork;

    public MarketplacePaymentService(
        IStockReservationRepository reservationRepository,
        IMarketplacePaymentRepository paymentRepository,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork)
    {
        this.reservationRepository = reservationRepository;
        this.paymentRepository = paymentRepository;
        this.outboxRepository = outboxRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<MarketplacePaymentResult> CreatePaymentIntentAsync(StockReservationId reservationId, CancellationToken ct)
    {
        var reservation = await GetReservationAsync(reservationId, ct);
        var existingPayment = await paymentRepository.GetByReservationIdAsync(reservationId, ct);

        if (existingPayment is not null)
        {
            return ToResult(existingPayment.Reservation ?? reservation, existingPayment);
        }

        if (reservation.Status != StockReservationStatus.Active)
        {
            throw new ValidationException($"Резерв находится в статусе {reservation.Status} и не может быть оплачен.");
        }

        if (reservation.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            ReleaseReservation(reservation, expired: true);
            await unitOfWork.SaveChangesAsync(ct);
            throw new ValidationException("Резерв истек. Создайте новый резерв для оплаты.");
        }

        var variant = reservation.ProductVariant;

        if (variant is null)
        {
            throw new ValidationException("Данные варианта товара недоступны для оплаты.");
        }

        var amount = variant.PriceAmount * reservation.Quantity;
        var idempotencyKey = $"marketplace-reservation:{reservation.Id}";
        var payment = MarketplacePayment.Create(reservation.Id, amount, variant.Currency, idempotencyKey);
        var message = new PaymentIntentRequestedMessage(
            reservation.Id.ToString(),
            amount,
            variant.Currency,
            idempotencyKey);

        await paymentRepository.PendingAddAsync(payment, ct);
        await outboxRepository.AddAsync(
            RabbitOutboxMessage.Create(
                PaymentIntentRequestedMessage.ExchangeName,
                PaymentIntentRequestedMessage.DefaultRoutingKey,
                CommonJsonSerializer.Serialize(message)),
            ct);
        await unitOfWork.SaveChangesAsync(ct);

        return ToResult(reservation, payment);
    }

    public async Task ApplyBankPaymentStatusAsync(
        StockReservationId reservationId,
        string paymentIntentId,
        string status,
        decimal amount,
        string currency,
        string? failureReason,
        CancellationToken ct)
    {
        var reservation = await reservationRepository.GetDetailsAsync(reservationId, ct);

        if (reservation is null)
        {
            throw new NotFoundException("Резерв не найден.");
        }

        var payment = await paymentRepository.GetByReservationIdAsync(reservationId, ct);

        if (payment is null)
        {
            throw new NotFoundException("Оплата не найдена.");
        }

        if (reservation.Status == StockReservationStatus.Active && reservation.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            ReleaseReservation(reservation, expired: true);
            payment.MarkExpired("Резерв истек до ответа банка.");
            await unitOfWork.SaveChangesAsync(ct);
            return;
        }

        payment.ApplyBankStatus(paymentIntentId, status, failureReason);

        if (payment.Status == MarketplacePaymentStatus.Paid)
        {
            reservation.MarkPaid();
        }
        else if (payment.Status == MarketplacePaymentStatus.Failed)
        {
            ReleaseReservation(reservation, expired: false);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<StockReservation> GetReservationAsync(StockReservationId reservationId, CancellationToken ct)
    {
        var reservation = await reservationRepository.GetDetailsAsync(reservationId, ct);

        if (reservation is null)
        {
            throw new NotFoundException("Резерв не найден.");
        }

        return reservation;
    }

    private static MarketplacePaymentResult ToResult(StockReservation reservation, MarketplacePayment payment)
    {
        return new MarketplacePaymentResult(
            reservation,
            payment.BankPaymentIntentId,
            payment.Status.ToString(),
            payment.Amount,
            payment.Currency,
            payment.FailureReason);
    }

    private static void ReleaseReservation(StockReservation reservation, bool expired)
    {
        if (reservation.Status != StockReservationStatus.Active)
        {
            return;
        }

        reservation.ProductVariant?.Inventory?.TryReleaseReserved(reservation.Quantity);

        if (expired)
        {
            reservation.Expire();
        }
        else
        {
            reservation.Release();
        }
    }
}
