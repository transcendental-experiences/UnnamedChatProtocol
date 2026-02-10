using System.Net;

namespace Ucp.Server.Gateway;

public static class GatewayApi
{
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
                using var sock = await context.WebSockets.AcceptWebSocketAsync();
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }).WithName("GatewayConnect");
    }

    public sealed record GetGatewayResponse(string Url);
}