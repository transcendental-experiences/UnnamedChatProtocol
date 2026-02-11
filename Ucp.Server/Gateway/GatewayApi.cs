using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ucp.Server.Gateway;

public static class GatewayApi
{
    public const string GATEWAY_CONFIG_SECT = "UcpGateway";
    
    
    public static void Map(WebApplication app)
    {
        app.MapGet("/gateway", async (HttpContext context, LinkGenerator linkGenerator) =>
        {
            var uri = linkGenerator.GetUriByName(context, "GatewayConnect");

            if (uri == null)
                throw new InvalidOperationException("Unable to figure out gateway URL!");

            if (uri.StartsWith("http://"))
                uri = "ws://" + uri["http://".Length..];
            else if (uri.StartsWith("https://"))
                uri = "wss://" + uri["https://".Length..];

            await context.Response.WriteAsJsonAsync(
                new GetGatewayResponse(uri));
        }).WithName("GetGateway");

        app.MapGet($"/__gateway_connect_{Random.Shared.Next(0, 99999)}", async (HttpContext context) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                // NOTE: When we have an actual gateway "chat" loop here we should probably spin up an entire new task.
                using var sock = await context.WebSockets.AcceptWebSocketAsync();
                var interval = app.Configuration.GetValue<int>($"{GATEWAY_CONFIG_SECT}:HeartbeatInterval");
                var msg = MakeGatewayMessage(GatewayOpcode.Hello, new GatewayHelloMessage(interval));
                await sock.SendAsync(new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(msg)), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }).WithName("GatewayConnect");
    }

    public static GatewayMessage MakeGatewayMessage(GatewayOpcode opcode, object gatewayMessage)
    {
        var payload = JsonSerializer.SerializeToNode(gatewayMessage);
        var msg = new GatewayMessage((int)opcode, payload!);
        
        return msg;
    }

    public sealed record GetGatewayResponse([property: JsonPropertyName("heartbeat_interval")] string Url);

    // TODO: It'd be nice if we could use discriminator based deserialization for the opcode.
    public sealed class GatewayMessage
    {
        public GatewayMessage(int opCode, JsonNode data)
        {
            OpCode = opCode;
            Data = data;
        }

        [JsonPropertyName("op")]
        public int OpCode { get; set; }
        [JsonPropertyName("d")]
        public JsonNode Data { get; set; }
        [JsonPropertyName("s")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SequenceNumber { get; set; }
        [JsonPropertyName("t")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? EventType { get; set; }
    }
    
    
    
    public class GatewayHelloMessage(int heartbeatInterval)
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; } = heartbeatInterval;
    }

    public enum GatewayOpcode
    {
        /// <summary>
        ///     Client &lt;-&gt; Server connection keepalives and latency measurement.
        /// </summary>
        Heartbeat = 1,
        /// <summary>
        ///     Server -&gt; Client self-introduction setting up heartbeats.
        /// </summary>
        Hello = 10,
    }
}