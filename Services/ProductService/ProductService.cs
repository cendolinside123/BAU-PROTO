namespace BAU_PROTO.Services.ProductService
{
    public interface ProductService
    {
        public Task<int> CreateProduct(AddProductDto createProductDto, string refreshToken);
        public Task<int> UpdateProduct(UpdateProducDto updateProducDto, string refreshToken);
        public Task<int> DeleteProduct(DeleteProductDto deleteProductDto, string refreshToken);
        public Task<List<Products>> GetProducts(SearchProductDto? searchProductDto, string refreshToken);
        public Task<Products> GetProductSpesific(SpesificSelectProductDto spesificSelectProductDto, string refreshToken);
    }
}
