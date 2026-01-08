using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.Application.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<IQueryable<Product>> GetProductsAsync()
    {
        return Task.FromResult(_context.Products.AsQueryable());
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        // Soft delete - just mark as deleted
        product.IsDeleted = true;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductDto>> GetProductsWithDetailsAsync(string? searchTerm = null, int? categoryId = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted) // Filter out deleted products
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(searchTerm) || 
                p.Sku.Contains(searchTerm) ||
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                ImportPrice = p.ImportPrice,
                Count = p.Count,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                Images = p.Images
            })
            .ToListAsync();
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetProductDtoByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                ImportPrice = p.ImportPrice,
                Count = p.Count,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                Images = p.Images
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto> CreateProductFromDtoAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Sku = dto.Sku,
            Name = dto.Name,
            ImportPrice = dto.ImportPrice,
            Count = dto.Count,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            Images = dto.Images
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await GetProductDtoByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to create product");
    }

    public async Task<ProductDto> UpdateProductFromDtoAsync(UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(dto.Id);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {dto.Id} not found");

        product.Sku = dto.Sku;
        product.Name = dto.Name;
        product.ImportPrice = dto.ImportPrice;
        product.Count = dto.Count;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.Images = dto.Images;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return await GetProductDtoByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to update product");
    }
}
