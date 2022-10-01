using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpClientFactory cartService;

        public CouponService(IHttpClientFactory cartService) : base(cartService)
        {
            this.cartService = cartService;
        }

        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                AccessToken = token,
                ApiType = SD.ApiType.GET,
                Data = couponCode,
                ApiUrl = SD.CouponApiBase + "/api/coupon?couponCode=" + couponCode
            });
        }
    }
}
