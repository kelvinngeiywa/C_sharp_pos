using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_pos.Models.DTOs;
using my_pos.Services.IServices;

namespace my_pos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productServices) : ControllerBase
    {
        private readonly ResponseDTO response = new ResponseDTO();

        [HttpGet]
        public async Task<ActionResult<ResponseDTO>> GetAllProducts()
        {
            var products = await productServices.GetAllProducts();

            if (products is null || !products.Any())
            {
                response.Success = false;
                response.Message = "No product found!!";
                response.Data = products;

                return NotFound(response);
            }

            response.Success = true;
            response.Message = "Products found!!";
            response.Data = products;

            return Ok(response);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseDTO>> AddProductAsync(AddproductDTO addproductDTO)
        {
            var newProduct= await productServices.AddProductAsync(addproductDTO);
            
            if(newProduct is null)
            {
                response.Success = false;
                response.Message = "product add failed!!!";
                response.Data = newProduct;
                return BadRequest(response);
            }

            if(newProduct.Name == addproductDTO.Name)
            {
                response.Success = true;
                response.Message = "Product exists, its count has been updated";
                response.Data = newProduct;

                return Ok(response);
            }

            response.Success = true;
            response.Message = "Product added successful1!";
            response.Data = newProduct;

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDTO>> GetProductById(Guid id)
        {
            var product = await productServices.GetProductById(id);

            if (product is null)
            {
                response.Success = false;
                response.Message = $"Product with id {id} not found!!!";
                response.Data = product;

                return NotFound(response);
            }

            response.Success = true;
            response.Message = "Product found!!!";
            response.Data = product;

            return Ok(response);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO>> UpdateProductAsync(Guid id,AddproductDTO updateProduct)
        {
            var results = await productServices.UpdateProductAsync(id, updateProduct);

            if (results is null)
            {
                response.Success = false;
                response.Message = "Product not found!!";
                response.Data = results;

                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "Product updated successfully!!!";
            response.Data = results;

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDTO>> DeleteProductAsync(Guid id)
        {
            var product = await productServices.DeleteProductAsync(id);

            if (product is null)
            {
                response.Success = false;
                response.Message = $"Product with id {id} not found!!";
                response.Data= product;

                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "Product deleted successfully!!";

            return Ok(response);
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("remove/{id}")]
        public async Task<ActionResult<ResponseDTO>> RemoveProductAsync(Guid id)
        {
            var results = await productServices.DeleteProduct(id);

            if (results is null)
            {
                response.Success = false;
                response.Message = "Product not found!!";
                response.Data = results;

                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "Product deleted successfully!!";
            response.Data = results;

            return Ok(response);

        }
    }
}
