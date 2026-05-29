using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("[action]")]
    public JsonDocument Get()
    {
        var value = new
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe"
        };
        var json = JsonSerializer.Serialize(value);
        return JsonDocument.Parse(json);
    }

    [HttpPost("[action]")]
    public JsonDocument Post([FromBody] JsonDocument jsonDocument)
    {
        var string2 = jsonDocument.RootElement.GetRawText();
        var node = JsonNode.Parse(string2);
        var jsonObject = node.AsObject();
        jsonObject["age"] = 30; 
        jsonObject["age1"] = 31;
        var newYork = "New york";
        jsonObject.Add("city", "New York");
        jsonObject.Add("city1", newYork);

        var jsonObject1 = JsonNode.Parse(string2);
        
        jsonObject.Add("nested", jsonObject1);
        
        var updatedJson = jsonObject.ToJsonString();
        var res = JsonDocument.Parse(updatedJson);
        return res;
    }

    [HttpPost("[action]")]
    public Dictionary<string, JsonElement?> Dict([FromBody] Dictionary<string, JsonElement?> dict)
    {
        var value = dict.GetValueOrDefault("additionalProp3");
        return dict;
    }
}