using System.Buffers;
using System.Buffers.Text;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Ucp.Server;
using Ucp.Server.Gateway;

namespace Ucp.Testing;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class IntegrationTestFixture
{
    protected WebApplicationFactory<Program> Factory;
    private const int MAX_GATEWAY_RECEIVE_SIZE = 16384;
    
    [SetUp]
    public async Task Setup()
    {
        Factory = new WebApplicationFactory<Program>();
    }

    [TearDown]
    public async Task Teardown()
    {
        await Factory.DisposeAsync();
    }
    
    public async Task<Uri> GetGateway(HttpClient client)
    {
        var response = await client.GetFromJsonAsync<GatewayApi.GetGatewayResponse>("/gateway");
        
        Assert.That(response?.Url, Is.Not.Null);
        Assert.That(response.Url, Does.StartWith("ws://").Or.StartsWith("wss://"));

        var uri = new Uri(response.Url);

        return uri;
    }

    public async Task<string> ReceiveGatewayMessage(WebSocket client, CancellationToken cancellationToken)
    {
        var receiveBuffer =
            new Memory<byte>(ArrayPool<byte>.Shared.Rent(MAX_GATEWAY_RECEIVE_SIZE), 0, MAX_GATEWAY_RECEIVE_SIZE);
        
        ValueWebSocketReceiveResult res;
        var loc = 0;
        
        do
        {
            res = await client.ReceiveAsync(receiveBuffer[loc..], cancellationToken);
            loc += res.Count;
        } while (!res.EndOfMessage);

        Assert.That(res.MessageType, Is.EqualTo(WebSocketMessageType.Text));
        var utf8Msg = receiveBuffer.Span[0..res.Count];

        return Encoding.UTF8.GetString(utf8Msg);
    }

    public async Task<WebSocket> OpenGateway(HttpClient client, CancellationToken cancellationToken)
    {
        var gatewayUrl = await GetGateway(client);

        var wsClient = Factory.Server.CreateWebSocketClient();
        
        var conn = await wsClient.ConnectAsync(gatewayUrl, cancellationToken);

        return conn;
    }
}