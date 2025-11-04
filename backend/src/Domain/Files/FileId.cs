using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Files;

public class FileId : GuidIdentifier<FileId>, IIdentifierDescription
{
    public FileId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Файл";
    public static string Prefix => "file";
}
