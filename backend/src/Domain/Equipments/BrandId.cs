using FitHub.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class BrandId : GuidIdentifier<BrandId>, IIdentifierDescription
{
    public BrandId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Брэнд";
    public static string Prefix => "brand";
}
