using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface IProductService : IBaseService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);

        Task<T> Create<T>(ProductDto productDto, string token);
        Task<T> Update<T>(ProductDto productDto, string token);
        Task<T> Delete<T>(int id, string token);

    }
}
