using my_pos.Models;
using my_pos.Models.DTOs;

namespace my_pos.Services.IServices
{
    public interface IProduct
    {
        Task<Product> AddProductAsync(AddproductDTO addproductDTO);
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProductById(Guid Id);
        Task<string> DeleteProductAsync(Guid Id);
    }
}
