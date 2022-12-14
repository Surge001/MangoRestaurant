using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(IHttpClientFactory httpClient): base(httpClient)
        {

        }

        public async Task<T> Create<T>(ProductDto productDto, string token)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                ApiUrl = SD.ProductApiBase + "/api/products",
                AccessToken = token
            });
        }

        public async Task<T> Delete<T>(int id, string token)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                ApiUrl = SD.ProductApiBase + "/api/products/" + id,
                AccessToken = token
            });
        }

        public async Task<T> GetAllAsync<T>(string token)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.ProductApiBase + "/api/products",
                AccessToken = token
            });
        }

        public async Task<T> GetAsync<T>(int id, string token)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.ProductApiBase + "/api/products/" + id,
                AccessToken = token
            });
        }
        public async Task<T> Update<T>(ProductDto productDto, string token)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                ApiUrl = SD.ProductApiBase + "/api/products",
                AccessToken = token
            });
        }
    }
}
