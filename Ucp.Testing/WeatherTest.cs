using System.Text.Json.Nodes;

namespace Ucp.Testing;

public sealed class WeatherTest : IntegrationTestFixture
{
    [Test]
    public async Task ForecastWorks()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/weatherforecast");

        var parsed = await JsonNode.ParseAsync(await response.Content.ReadAsStreamAsync());
        
        Assert.That(parsed, Is.Not.Null);
        
        Assert.That(parsed, Is.AssignableTo<JsonArray>());
        Assert.That(parsed[0], Is.AssignableTo<JsonObject>());
        
        Assert.That(parsed[0], Has.ItemAt("date").Not.Null);
        Assert.That(parsed[0], Has.ItemAt("temperatureC").Not.Null);
        Assert.That(parsed[0], Has.ItemAt("summary").Not.Null);
        Assert.That(parsed[0], Has.ItemAt("temperatureF").Not.Null);
    }
}