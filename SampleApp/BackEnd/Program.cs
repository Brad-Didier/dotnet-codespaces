using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    // current workaround for port forwarding in codespaces
    // https://github.com/dotnet/aspnetcore/issues/57332
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/WeatherForecastByCity/{city}", (string city) =>
{
    /// <summary>
    /// Generates an array of <see cref="WeatherForecastByCity"/> objects with random temperature and summary values for a specific city.
    /// </summary>
    /// <param name="city">The name of the city for which the weather forecast is generated.</param>
    /// <returns>
    /// An array of <see cref="WeatherForecastByCity"/> objects, each representing a weather forecast for a specific day in the given city.
    /// </returns>
    /// <remarks>
    /// The method creates 5 weather forecasts starting from the current date, each with:
    /// - A date incremented by the index (1 to 5).
    /// - A random temperature between -20 and 55 degrees.
    /// - A random summary selected from the <c>summaries</c> array.
    /// </remarks>
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecastByCity
        (
            city,
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecastByCity");

app.MapGet("/weatherforecast", () =>
{
    /// <summary>
    /// Generates an array of <see cref="WeatherForecast"/> objects with random temperature and summary values.
    /// </summary>
    /// <returns>
    /// An array of <see cref="WeatherForecast"/> objects, each representing a weather forecast for a specific day.
    /// </returns>
    /// <remarks>
    /// The method creates 5 weather forecasts starting from the current date, each with:
    /// - A date incremented by the index (1 to 5).
    /// - A random temperature between -20 and 55 degrees.
    /// - A random summary selected from the <c>summaries</c> array.
    /// </remarks>
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
internal record WeatherForecastByCity(string City, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}