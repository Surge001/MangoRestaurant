using Mango.Services.ProductApi.Models.Dto;
using Mango.Services.ProductApi.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/products")]
    public class ProductApiController : ControllerBase
    {
        protected ResponseDto response;

        private readonly IProductRepository productRepository;

        public ProductApiController(IProductRepository repository)
        {
            this.productRepository = repository;
            this.response = new ResponseDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductDto> result = await this.productRepository.GetProducts();
                this.response.Result = result;
            }
            catch(Exception ex)
            {
                this.response.IsSuccess = false;
                this.response.ErrorMessages = new List<string>() { ex.Message + Environment.NewLine + ex.StackTrace };
            }
            return this.response;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
                ProductDto result = await this.productRepository.GetProductById(id);
                this.response.Result = result;
            }
            catch (Exception ex)
            {
                this.response.IsSuccess = false;
                this.response.ErrorMessages = new List<string>() { ex.Message + Environment.NewLine + ex.StackTrace };
            }
            return this.response;
        }
    }
}
