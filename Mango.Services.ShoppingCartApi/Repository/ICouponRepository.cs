using System.Threading.Tasks;
using Mango.Services.ShoppingCartApi.Model.Dto;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> Get(string couponName);
    }
}
