using Microsoft.EntityFrameworkCore;
using my_pos.Data;
using my_pos.Models;
using my_pos.Models.DTOs;
using my_pos.Services.IServices;
using System.Security.Claims;

namespace my_pos.Services
{
    public class ProductServices(AppDbContext dbContext, 
        IHttpContextAccessor _contextAccessor,
        ILogger<ProductServices> _logger) : IProduct
    {
        public async Task<Product> AddProductAsync(AddproductDTO addproductDTO)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //_logger.LogError($"Id is {userId}");
            var existingproduct = await dbContext.products.FirstOrDefaultAsync(u => u.Name == addproductDTO.Name);
            //Console.WriteLine(existingproduct);
            if (existingproduct != null)
            {
                //Console.WriteLine($"Product with name {addproductDTO.Name} exists");
                
                existingproduct.Count += addproductDTO.Count;
                await dbContext.SaveChangesAsync();
                return existingproduct;

            }

            var newproduct = new Product {
                Name= addproductDTO.Name,
                price=addproductDTO.Price,
                Count=addproductDTO.Count,
                DateAdded=DateTime.UtcNow,
                AddedBy=userId.ToString(),
                IsDeleted=false
            };

            dbContext.products.Add(newproduct);
            await dbContext.SaveChangesAsync();
            return newproduct;
        }

        public async Task<string> DeleteProduct(Guid Id)
        {
            var product = await dbContext.products.FindAsync(Id);

            if (product is null)
            {
                return null;

            }

            dbContext.Remove(product);
            await dbContext.SaveChangesAsync();
            return "";
        }

        public async Task<string> DeleteProductAsync(Guid Id)
        {
            var product = await dbContext.products.FirstOrDefaultAsync(p => p.Id == Id);

            if (product is null){
                return null;

            }

            product.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return "Product deleted successfully!!";


        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await dbContext.products.Where(p=>p.IsDeleted == false).ToListAsync();
        }

        public async Task<Product> GetProductById(Guid Id)
        {
            var product = await dbContext.products.FindAsync(Id);
             if(product is null)
            {
                return null;
            }
            return product;
        }

        public async Task<Product> UpdateProductAsync(Guid Id, AddproductDTO updateProductDTO)
        {
            var product = await dbContext.products.FindAsync(Id);
            if (product is null)
            {
                return null;
            }
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            product.Name= updateProductDTO.Name;
            product.price = updateProductDTO.Price;
            product.Count= updateProductDTO.Count;
            product.UpdatedBy = userId.ToString();

            await dbContext.SaveChangesAsync();
            return product;

        }
    }
}
