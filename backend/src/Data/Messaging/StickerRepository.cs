using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

public class StickerRepository : DefaultPendingRepository<Sticker, StickerId, DataContext>, IStickerRepository
{
    public StickerRepository(DataContext context) : base(context)
    {
    }
}
