using Inventory.Api.DTOs;
using Inventory.Api.Models;

namespace Inventory.Api.Services;

public class ProductService
{
    private readonly List<Product> _products = new List<Product>
    {
        new Product
        {
            Id = Guid.NewGuid(),
            Name = "Camiseta preta",
            Description = "Camiseta basica tamanho G",
            Price = 59.90m,
            StockQuantity = 3,
            CreatedAt = DateTime.UtcNow
        },
        new Product
        {
            Id = Guid.NewGuid(),
            Name = "Calça Jeans",
            Description = "Calça Jeans azul tamanho P",
            Price = 89.90m,
            StockQuantity = 3,
            CreatedAt = DateTime.UtcNow
        },
    };
    public List<Product> GetAll(string? search)
    {
        if(string.IsNullOrWhiteSpace(search))
        {
            return _products.ToList();
        }
        return _products
        .Where(product => product.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
        .ToList();
    }
    public Product? GetById(Guid id)
    {
        return _products.FirstOrDefault(product => product.Id == id);
    }
    public Product Create(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };
        _products.Add(product);
        return product;
    }
    public Product? Update(Guid id, UpdateProductRequest request)
    {
        var product = GetById(id);
        if (product is null)
        {
            return null;
        }
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        return product;
    }
    public bool Delete(Guid id)
    {
        var product = GetById(id);
        if (product is null)
        {
            return false;
        }
        _products.Remove(product);
        return true;
    }
}