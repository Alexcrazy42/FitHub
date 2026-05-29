namespace FitHub.Application.Marketplace;

public static class MarketplaceCatalogSort
{
    public const string Popular = "popular";
    public const string Newest = "newest";
    public const string PriceAsc = "price_asc";
    public const string PriceDesc = "price_desc";

    public static string Normalize(string? sort)
    {
        return sort switch
        {
            Newest => Newest,
            PriceAsc => PriceAsc,
            PriceDesc => PriceDesc,
            _ => Popular
        };
    }
}
