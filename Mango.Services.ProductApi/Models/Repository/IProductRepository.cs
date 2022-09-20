using Mango.Services.ProductApi.Models.Dto;

namespace Mango.Services.ProductApi.Models.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProducts();

        Task<ProductDto> GetProductById(int productId);

        Task<ProductDto> CreateUpdateProducct(ProductDto product);

        Task<bool> DeleteProduct(int productId);

    }
}
