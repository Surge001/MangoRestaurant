using Mango.Service.CouponApi.Model.Dto;
using Mango.Service.CouponApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Mango.Service.CouponApi.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository repository;

        private ResponseDto responseDto;
        public CouponController(ICouponRepository repository)
        {
            this.repository = repository;
            this.responseDto = new ResponseDto();
        }

        [HttpGet("couponCode")]
        public async Task<object> GetDiscountForCode(string couponCode)
        {
            try
            {
                CouponDto dto = await this.repository.GetByCode(couponCode);
                this.responseDto.Result = dto;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };
            }
            return this.responseDto;
        }
    }
}
