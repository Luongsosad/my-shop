using my_shop.Domain.Entities;

namespace my_shop.Application.Interfaces;

public interface IProductService
{
    Task<IQueryable<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
}
