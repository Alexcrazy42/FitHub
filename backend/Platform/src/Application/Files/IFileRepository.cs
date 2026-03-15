using FitHub.Common.Entities.Storage;
using FitHub.Domain.Files;

namespace FitHub.Application.Files;

public interface IFileRepository : IPendingRepository<FileEntity, FileId>
{

}
