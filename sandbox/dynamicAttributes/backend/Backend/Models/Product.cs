using System.Text.Json.Nodes;

namespace Backend.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public JsonNode? Attributes { get; set; }
    public List<ProductCategory> Categories { get; set; } = new();
}
