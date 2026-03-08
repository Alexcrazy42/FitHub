using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

public class StickerGroupRepository : DefaultPendingRepository<StickerGroup, StickerGroupId, DataContext>, IStickerGroupRepository
{
    public StickerGroupRepository(DataContext context) : base(context)
    {
    }
}
