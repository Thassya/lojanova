using LojaNova.Shared.Models;

namespace LojaNova.Ecommerce.Api.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product, Stream? imageStream = null, string? imageFileName = null);
        Task<bool> UpdateProductAsync(Product product, Stream? imageStream = null, string? imageFileName = null);
        Task<bool> DeleteProductAsync(int id);
    }
}
