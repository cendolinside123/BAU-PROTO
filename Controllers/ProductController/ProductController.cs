using BAU_PROTO.Services.AuthService;
using BAU_PROTO.Services.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BAU_PROTO.Controllers.ProductController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ProductService _productService;
        private readonly AuthService _authService;

        public ProductController(AppDbContext context)
        {
            _context = context;
            _productService = new ProductServiceImpl(_context);
            _authService = new AuthServiceImpl(_context);
        }


        [HttpPost("AddProduct")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<Object> AddProduct(AddProductDto addProductDto)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _authService.TokenValidation(accessToken);
                var result = await _productService.CreateProduct(addProductDto);
                var response = new
                {
                    message = "add product success"
                };
                return Ok(response);
            }
            catch (JwtException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _authService.TokenValidation(accessToken);
                _ = await _productService.UpdateProduct(updateProducDto);
                var response = new
                {
                    message = "update product success"
                };
                return Ok(response);
            }
            catch (JwtException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _authService.TokenValidation(accessToken);
                _ = await _productService.DeleteProduct(deleteProductDto);
                var response = new
                {
                    message = "delete product success"
                };
                return Ok(response);
            }
            catch (JwtException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _authService.TokenValidation(accessToken);
                var result = await _productService.GetProducts(searchProductDto);
                var response = new
                {
                    message = "get list product success",
                    count = result.Count(),
                    data = result
                };
                return Ok(response);
            }
            catch (JwtException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _authService.TokenValidation(accessToken);
                var result = await _productService.GetProductSpesific(searchProductDto);
                var response = new
                {
                    message = "get product success",
                    data = result
                };
                return Ok(response);
            }
            catch (JwtException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}   
