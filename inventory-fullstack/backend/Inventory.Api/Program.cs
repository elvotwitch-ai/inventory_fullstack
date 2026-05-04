using Inventory.Api.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var products = new List<Product>
{
    new Product
    {
        Id = Guid.NewGuid(),
        Name = "camiseta preta",
        Description = "camiseta basica tamanho G",
        Price = 59.90m,
        StockQuantity = 3,
        CreatedAt = DateTime.UtcNow
    },
    new Product
    {
        Id = Guid.NewGuid(),
        Name = "calça jeans",
        Description = "calça jeans azul tamanho P",
        Price = 89.90m,
        StockQuantity = 3,
        CreatedAt = DateTime.UtcNow
    }
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

app.MapGet("/healt", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        message = "Inventory API is running"
    });
});

app.MapGet("/products", () =>
{
    return Results.Ok(products);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
