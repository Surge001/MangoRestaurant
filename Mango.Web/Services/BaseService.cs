using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="httpClient">Instance of http client factory</param>
        public BaseService(IHttpClientFactory httpClient)
        {
            this.HttpClient = httpClient;
        }

        #endregion

        public ResponseDto ResponseModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IHttpClientFactory HttpClient { get; private set; }

        public async Task<T> SendAsync<T>(ApiRequest request)
        {
            var httpClient = this.HttpClient.CreateClient("MangoApi");
            using (HttpRequestMessage requestMessage = new HttpRequestMessage())
            {
                requestMessage.Headers.Add("Accept", "application/json");
                requestMessage.RequestUri = new Uri(request.ApiUrl);
                httpClient.DefaultRequestHeaders.Clear();
                if (request.Data != null)
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                    requestMessage.Content = content;
                }
                switch (request.ApiType)
                {
                    case SD.ApiType.POST:
                        requestMessage.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        requestMessage.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        requestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        requestMessage.Method = HttpMethod.Get;
                        break;
                }
                using (HttpResponseMessage apiResponse = await httpClient.SendAsync(requestMessage))
                {
                    string apiResponseContent = await apiResponse.Content.ReadAsStringAsync();

                    if (apiResponse.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<T>(apiResponseContent);
                    else
                        throw new ApplicationException(apiResponseContent);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
