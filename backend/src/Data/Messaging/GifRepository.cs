using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

public class GifRepository : DefaultPendingRepository<Gif, GifId, DataContext>, IGifRepository
{
    public GifRepository(DataContext context) : base(context)
    {
    }
}
