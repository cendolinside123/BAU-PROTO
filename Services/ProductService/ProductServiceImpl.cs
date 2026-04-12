using BAU_PROTO.Persistence;
using BAU_PROTO.Services.AuthService;
using BAU_PROTO.Services.JwtService;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BAU_PROTO.Services.ProductService
{
    public class ProductServiceImpl : ProductService
    {

        private readonly AppDbContext _context;
        private readonly AuthServiceImpl _authService;

        public ProductServiceImpl(AppDbContext context)
        {
            _context = context;
            _authService = new AuthServiceImpl(context);
            
        }
        public async Task<int> CreateProduct(AddProductDto createProductDto, string refreshToken)
        {
            try
            {
                _ = _authService.RefreshTokenValidation(refreshToken);

                var validationErrors = createProductDto.InputValidation();

                if (validationErrors.Count > 0)
                {
                    throw new ArgumentException(string.Join(", ", validationErrors));
                }

                var product = new Products
                {
                    Name = createProductDto.Name,
                    Description = createProductDto.Description,
                    Price = createProductDto.GetPrice()
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product.Id;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred: ", ex);
            }
        }

        public async Task<int> DeleteProduct(DeleteProductDto deleteProductDto, string refreshToken)
        {
            try
            {
                _ = _authService.RefreshTokenValidation(refreshToken);

                var validationErrors = deleteProductDto.InputValidation();

                if (validationErrors.Count > 0)
                {
                    throw new ArgumentException(string.Join(", ", validationErrors));
                }

                var product = await _context.Products.FirstOrDefaultAsync(u => u.Id == deleteProductDto.GetId());
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    return product.Id; 
                } else
                {
                    throw new ArgumentException("Already deleted");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred: ", ex);
            }
        }

        public async Task<List<Products>> GetProducts(SearchProductDto searchProductDto, string refreshToken)
        {
            try
            {
                _ = _authService.RefreshTokenValidation(refreshToken);
                IQueryable<Products> productQuery = _context.Products;

                if (!string.IsNullOrEmpty(searchProductDto.Name))
                {
                    productQuery = productQuery.Where(p => p.Name.Contains(searchProductDto.Name));
                }

                if (searchProductDto.page.HasValue && searchProductDto.pageSize.HasValue)
                {
                    productQuery = productQuery
                        .Skip((searchProductDto.page.Value - 1) * searchProductDto.pageSize.Value)
                        .Take(searchProductDto.pageSize.Value);
                }

                var results = await productQuery.ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred: ", ex);
            }
        }

        public async Task<int> UpdateProduct(UpdateProducDto updateProducDto, string refreshToken)
        {
            try
            {
                _ = _authService.RefreshTokenValidation(refreshToken);
                var validationErrors = updateProducDto.InputValidation();

                if (validationErrors.Count > 0)
                {
                    throw new ArgumentException(string.Join(", ", validationErrors));
                }

                var product = await _context.Products.FirstOrDefaultAsync(u => u.Id == updateProducDto.GetId());

                if (product != null)
                {
                    product.Price = updateProducDto.GetPrice();
                    product.Name = updateProducDto.Name;
                    product.Description = updateProducDto.Description;
                    product.UpdatedAt = DateTime.UtcNow;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    return product.Id;
                }
                else
                {
                    throw new ArgumentException("Product not found");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred: ", ex);
            }
        }

        public async Task<Products> GetProductSpesific(SpesificSelectProductDto spesificSelectProductDto, string refreshToken)
        {
            try
            {
                _ = _authService.RefreshTokenValidation(refreshToken);
                var validationErrors = spesificSelectProductDto.InputValidation();

                if (validationErrors.Count > 0)
                {
                    throw new ArgumentException(string.Join(", ", validationErrors));
                }

                var product = await _context.Products.FirstOrDefaultAsync(u => u.Id == spesificSelectProductDto.GetId());
                if (product != null)
                {
                    return product;
                }
                else
                {
                    throw new ArgumentException("Product not found");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred: ", ex);
            }
        }
    }
}
