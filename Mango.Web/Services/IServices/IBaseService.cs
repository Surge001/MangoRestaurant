using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        IHttpClientFactory HttpClient { get; }
        ResponseDto ResponseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest request);
    }
}
