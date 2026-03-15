using FitHub.Application.Files;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Files;

namespace FitHub.Data.Equipments.Files;

public class FileRepository : DefaultPendingRepository<FileEntity, FileId, DataContext>, IFileRepository
{
    public FileRepository(DataContext context) : base(context)
    {
    }
}
