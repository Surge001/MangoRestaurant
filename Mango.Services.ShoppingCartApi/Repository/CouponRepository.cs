using Mango.Services.ShoppingCartApi.Model.Dto;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient couponClient;

        public CouponRepository(HttpClient couponClient)
        {
            this.couponClient = couponClient;
        }

        public async Task<CouponDto> Get(string couponName)
        {
            HttpResponseMessage response = await this.couponClient.GetAsync($"/api/coupon/{couponName}");
            string content = await response.Content.ReadAsStringAsync();
            ResponseDto responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);
            if (responseDto.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseDto.Result));
            }
            else
            {
                return new CouponDto();
            }
        }
    }
}
