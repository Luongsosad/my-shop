using my_shop.Application.DTOs;
using my_shop.Domain.Entities;

namespace my_shop.Application.Interfaces;

public interface IProductService
{
    Task<IQueryable<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    
    // Enhanced operations
    Task<List<ProductDto>> GetProductsWithDetailsAsync(string? searchTerm = null, int? categoryId = null);
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<ProductDto?> GetProductDtoByIdAsync(int id);
    Task<ProductDto> CreateProductFromDtoAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductFromDtoAsync(UpdateProductDto dto);
}
