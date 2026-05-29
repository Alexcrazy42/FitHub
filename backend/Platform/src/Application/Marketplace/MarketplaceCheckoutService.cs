using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public class MarketplaceCheckoutService : IMarketplaceCheckoutService
{
    public static readonly TimeSpan ReservationTtl = TimeSpan.FromMinutes(15);

    private readonly IStockReservationRepository reservationRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentIdentityUserIdAccessor currentUserIdAccessor;

    public MarketplaceCheckoutService(
        IStockReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ICurrentIdentityUserIdAccessor currentUserIdAccessor)
    {
        this.reservationRepository = reservationRepository;
        this.unitOfWork = unitOfWork;
        this.currentUserIdAccessor = currentUserIdAccessor;
    }

    public async Task<StockReservation> CreateReservationAsync(CreateCheckoutReservationCommand command, CancellationToken ct)
    {
        if (command.Quantity <= 0)
        {
            throw new ValidationException("Количество должно быть больше нуля.");
        }

        if (String.IsNullOrWhiteSpace(command.IdempotencyKey))
        {
            throw new ValidationException("IdempotencyKey обязателен.");
        }

        var now = DateTimeOffset.UtcNow;
        var existingReservation = await reservationRepository.GetByIdempotencyKeyAsync(command.IdempotencyKey, ct);

        if (existingReservation is not null)
        {
            if (existingReservation.Status == StockReservationStatus.Active && existingReservation.ExpiresAt <= now)
            {
                ReleaseReservation(existingReservation);
                await unitOfWork.SaveChangesAsync(ct);
                throw new ValidationException("Резерв уже истек. Повторите оформление заказа.");
            }

            return existingReservation;
        }

        var variant = await reservationRepository.GetVariantForReservationAsync(command.ProductVariantId, ct);

        if (variant is null)
        {
            throw new NotFoundException("Вариант товара не найден.");
        }

        if (!MarketplaceVariantAvailability.IsAvailable(variant) ||
            variant.Inventory is null ||
            !variant.Inventory.TryReserve(command.Quantity))
        {
            throw new ValidationException("Товар закончился или уже зарезервирован другим пользователем.");
        }

        var reservation = StockReservation.Create(
            variant.Id,
            command.Quantity,
            now.Add(ReservationTtl),
            command.IdempotencyKey,
            currentUserIdAccessor.GetCurrentUserId());

        await reservationRepository.PendingAddAsync(reservation, ct);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (ConcurrencyException ex)
        {
            throw new ValidationException("Товар уже зарезервировали. Обновите страницу и попробуйте снова.", ex);
        }

        return reservation;
    }

    public Task<StockReservation?> GetReservationAsync(StockReservationId reservationId, CancellationToken ct)
    {
        return reservationRepository.GetDetailsAsync(reservationId, ct);
    }

    public async Task<int> ReleaseExpiredReservationsAsync(DateTimeOffset now, CancellationToken ct)
    {
        var reservations = await reservationRepository.GetExpiredActiveReservationsAsync(now, ct);

        if (reservations.Count == 0)
        {
            return 0;
        }

        foreach (var reservation in reservations)
        {
            ReleaseReservation(reservation);
        }

        try
        {
            return await unitOfWork.SaveChangesAsync(ct);
        }
        catch (ConcurrencyException ex)
        {
            throw new ValidationException("Не удалось освободить часть резервов из-за конкурентного изменения остатков.", ex);
        }
    }

    private static void ReleaseReservation(StockReservation reservation)
    {
        reservation.ProductVariant?.Inventory?.TryReleaseReserved(reservation.Quantity);
        reservation.Expire();
    }
}
