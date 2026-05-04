using Inventory.Api.DTOs;
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

app.MapGet("/products/{id:guid}", (Guid id) =>
{
    var product = products.FirstOrDefault(product => product.Id == id);

    if (product is null)
    {
        return Results.NotFound(new
        {
            message = "Product not found"
        });
    }

    return Results.Ok(product);
});

app.MapPost("/products", (CreateProductRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new
        {
            message = "Product name is required"
        });
    }
    if (request.Price <= 0)
    {
        return Results.BadRequest(new
        {
            message = "Product price must be greater than zero"
        });
    }
    if (request.StockQuantity < 0)
    {
        return Results.BadRequest(new
        {
            message = "Stock quantity cannot be negative"
        });
    }
    var product = new Product
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Description = request.Description,
        Price = request.Price,
        StockQuantity = request.StockQuantity,
        CreatedAt = DateTime.UtcNow
    };

    products.Add(product);
    return Results.Created($"/products/{product.Id}", product);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
