using Ucp.Server.Testing;

namespace Ucp.Server;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // Loadbearing weather forecast, see WeatherTest.
        // Remove when we have some properly tested endpoints.
        WeatherApi.Map(app);
        
        app.Run();
    }
}
