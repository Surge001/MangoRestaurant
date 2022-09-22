using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface IProductService : IBaseService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);

        Task<T> Create<T>(ProductDto productDto);
        Task<T> Update<T>(ProductDto productDto);
        Task<T> Delete<T>(int id);

    }
}
