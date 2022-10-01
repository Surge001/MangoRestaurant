using Mango.Service.CouponApi.Model.Dto;

namespace Mango.Service.CouponApi.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetByCode(string couponCode);
    }
}
