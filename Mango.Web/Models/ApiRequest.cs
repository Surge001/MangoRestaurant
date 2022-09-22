using static Mango.Web.SD;

namespace Mango.Web.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string ApiUrl { get; set; }

        /// <summary>
        /// Gets or sets data that is to be passed to targeted api operation.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets security token for authentication
        /// </summary>
        public string AccessToken { get; set; }

    }
}
