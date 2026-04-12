using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class MarketplaceBrand : IEntity<MarketplaceBrandId>
{
    private MarketplaceBrand(MarketplaceBrandId id, string name, string slug)
    {
        Id = id;
        Name = name;
        Slug = slug;
    }

    public MarketplaceBrandId Id { get; }
    public string Name { get; private set; }
    public string Slug { get; private set; }

    public static MarketplaceBrand Create(string name, string slug)
    {
        return new MarketplaceBrand(MarketplaceBrandId.New(), name, slug);
    }
}
