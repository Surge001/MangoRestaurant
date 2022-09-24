using Mango.Services.ProductApi.Models.Dto;
using Mango.Services.ProductApi.Models.Repository;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductDto> result = await this.productRepository.GetProducts();
                this.response.Result = result;
            }
            catch (Exception ex)
            {
                this.response.IsSuccess = false;
                this.response.ErrorMessages = new List<string>() { ex.Message + Environment.NewLine + ex.StackTrace };
            }
            return this.response;
        }

        [Authorize]
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

        [Authorize]
        [HttpPost]
        public async Task<object> Post([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto product = await this.productRepository.CreateUpdateProducct(productDto);
                this.response.Result = product;
            }
            catch (Exception ex)
            {
                this.response.IsSuccess = false;
                this.response.ErrorMessages = new List<string>() { ex.Message };
            }
            return this.response;
        }

        [Authorize]
        [HttpPut]
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto product = await this.productRepository.CreateUpdateProducct(productDto);
                this.response.Result = product;
            }
            catch (Exception ex)
            {
                this.response.IsSuccess = false;
                this.response.ErrorMessages = new List<string>() { ex.Message };
            }
            return this.response;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{productId}")]
        public async Task<object> Delete(int productId)
        {
            try
            {
                this.response.IsSuccess = await this.productRepository.DeleteProduct(productId);
            }
            catch (Exception ex)
            {
                this.response.IsSuccess = false;
                this.response.ErrorMessages = new List<string>() { ex.Message };
            }
            return this.response;
        }
    }
}
