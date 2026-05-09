using Inventory.Api.DTOs;
using Inventory.Api.Models;
using Inventory.Api.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProductService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//app.UseHttpsRedirection();
builder.Services.AddOpenApi();

var app = builder.Build();

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

app.MapGet("/products", (string? search, ProductService productService) =>
{
    var products = productService.GetAll(search);

    return Results.Ok(products);
});

app.MapGet("/products/{id:guid}", (Guid id, ProductService productService) =>
{
    var product = productService.GetById(id);
    if (product is null)
    {
        return Results.NotFound(new
        {
            message = "Product not Found"
        });
    }
    return Results.Ok(product);
});

app.MapPost("/products", (CreateProductRequest request, ProductService productService) =>
{
    var validationError = ValidateCreateProductRequest(request);
    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            message = validationError
        });
    }
    var product = productService.Create(request);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id:guid}", (Guid id, UpdateProductRequest request, ProductService productService) =>
{
    var validationError = ValidateUpdateProductRequest(request);
    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            message = validationError
        });
    }
    var product = productService.Update(id, request);
    if (product is null)
    {
        return Results.NotFound(new
        {
            message = "Product not found"
        });
    }

    return Results.Ok(product);
});

app.MapDelete("/products/{id:guid}", (Guid id, ProductService productService) =>
{
    var deleted = productService.Delete(id);
    if (!deleted)
    {
        return Results.NotFound(new
        {
            message = "Product not found"
        });
    }
    return Results.NoContent();
});

app.Run();

static string? ValidateCreateProductRequest(CreateProductRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return "Product name is required";
    }
    if (request.Price <= 0)
    {
        return "Product price must be grater than zero";
    }
    if (request.StockQuantity < 0)
    {
        return "Stock quantity cannot be negative";
    }
    return null;
}
static string? ValidateUpdateProductRequest(UpdateProductRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return "Product name is required";
    }

    if (request.Price <= 0)
    {
        return "Product price must be greater than zero";
    }

    if (request.StockQuantity < 0)
    {
        return "Stock quantity cannot be negative";
    }

    return null;
}
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
