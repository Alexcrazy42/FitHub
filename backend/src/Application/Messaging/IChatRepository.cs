using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IChatRepository : IPendingRepository<Chat, ChatId>
{

}
