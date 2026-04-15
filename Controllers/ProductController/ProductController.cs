using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using BAU_PROTO.Services.ProductService;

namespace BAU_PROTO.Controllers.ProductController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ProductService _productService;

        public ProductController(AppDbContext context)
        {
            _context = context;
            _productService = new ProductServiceImpl(_context);
        }


        [HttpPost("AddProduct")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<Object> AddProduct(AddProductDto addProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["Refreshtoken"].ToString();
                var result = await _productService.CreateProduct(addProductDto, refreshToken);
                var response = new
                {
                    message = "add product success"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("UpdateProduct")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<Object> EditProduct(UpdateProducDto updateProducDto)
        {
            try
            {
                var refreshToken = Request.Headers["Refreshtoken"].ToString();
                _ = await _productService.UpdateProduct(updateProducDto, refreshToken);
                var response = new
                {
                    message = "update product success"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("DeleteProduct")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<Object> DeleteProduct(DeleteProductDto deleteProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["Refreshtoken"].ToString();
                _ = await _productService.DeleteProduct(deleteProductDto, refreshToken);
                var response = new
                {
                    message = "delete product success"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }


        [HttpPost("GetListProduct")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<Object> GetListProduct(SearchProductDto? searchProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["Refreshtoken"].ToString();
                var result = await _productService.GetProducts(searchProductDto, refreshToken);
                var response = new
                {
                    message = "get list product success",
                    count = result.Count(),
                    data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("GetProduct")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<Object> GetProduct(SpesificSelectProductDto searchProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["Refreshtoken"].ToString();
                var result = await _productService.GetProductSpesific(searchProductDto, refreshToken);
                var response = new
                {
                    message = "get product success",
                    data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}   
