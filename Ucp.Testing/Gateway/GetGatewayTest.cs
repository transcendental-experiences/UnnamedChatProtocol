using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Ucp.Server.Gateway;

namespace Ucp.Testing.Gateway;

public sealed class GetGatewayTest : IntegrationTestFixture
{
    [Test]
    public async Task FindGatewayTest()
    {
        var client = Factory.CreateClient();

        var response = await client.GetFromJsonAsync<GatewayApi.GetGatewayResponse>("/gateway");
        
        Assert.That(response?.Url, Is.Not.Null);
        Assert.That(response.Url, Does.StartWith("ws://").Or.StartsWith("wss://"));

        {
            var uri = new Uri(response.Url);
            
            var failAtWebsockets = await client.GetAsync(uri.AbsolutePath);
            
            Assert.That(failAtWebsockets.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}