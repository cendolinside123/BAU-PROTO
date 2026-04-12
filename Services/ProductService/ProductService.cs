namespace BAU_PROTO.Services.ProductService
{
    public interface ProductService
    {
        public Task<int> CreateProduct(AddProductDto createProductDto);
        public Task<int> UpdateProduct(UpdateProducDto updateProducDto);
        public Task<int> DeleteProduct(int id);
    }
}
