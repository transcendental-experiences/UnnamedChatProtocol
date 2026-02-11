using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Ucp.Server.Gateway;

namespace Ucp.Testing.Gateway;

public sealed class GetGatewayTest : IntegrationTestFixture
{
    [Test]
    [CancelAfter(timeout: 5000)]
    public async Task OpenConnectionTest(CancellationToken cancellationToken)
    {
        var client = Factory.CreateClient();
        
        var gatewayUrl = await GetGateway(client);

        var wsClient = Factory.Server.CreateWebSocketClient();
        
        var conn = await wsClient.ConnectAsync(gatewayUrl, cancellationToken);
    }

    [Test]
    [CancelAfter(timeout: 5000)]
    public async Task ReceiveHelloTest(CancellationToken cancellationToken)
    {
        var client = Factory.CreateClient();
        
        var gateway = await OpenGateway(client, cancellationToken);

        var helloText = await ReceiveGatewayMessage(gateway, cancellationToken);
        await TestContext.Out.WriteLineAsync($"Hello: {helloText}");

        var jsonObj = (JsonObject)JsonNode.Parse(helloText)!;
        
        Assert.That(jsonObj, Does.Not.ContainKey("t"));
        Assert.That(jsonObj, Does.Not.ContainKey("s"));

        var helloInfo = JsonSerializer.Deserialize<GatewayApi.GatewayMessage>(helloText);
        
        Assert.That(helloInfo, Is.Not.Null);
        Assert.That(helloInfo!.OpCode, Is.EqualTo((int)GatewayApi.GatewayOpcode.Hello));

        var hello = helloInfo.Data.Deserialize<GatewayApi.GatewayHelloMessage>();
        
        Assert.That(hello, Is.Not.Null);
        Assert.That(hello.HeartbeatInterval, Is.Not.Zero);
    }
    
    
}