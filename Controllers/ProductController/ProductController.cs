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
        [Authorize]
        public ActionResult<Object> AddProduct(AddProductDto addProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                _ = _productService.CreateProduct(addProductDto, refreshToken);
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
        [Authorize]
        public ActionResult<Object> EditProduct(UpdateProducDto updateProducDto)
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                _ = _productService.UpdateProduct(updateProducDto, refreshToken);
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
        [Authorize]
        public ActionResult<Object> DeleteProduct(DeleteProductDto deleteProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                _ = _productService.DeleteProduct(deleteProductDto  , refreshToken);
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
        [Authorize]
        public ActionResult<Object> GetListProduct(SearchProductDto searchProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                var result = _productService.GetProducts(searchProductDto, refreshToken).Result;
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
        [Authorize]
        public ActionResult<Object> GetProduct(SpesificSelectProductDto searchProductDto)
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                var result = _productService.GetProductSpesific(searchProductDto, refreshToken).Result;
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
        {

        }
    }
}   
