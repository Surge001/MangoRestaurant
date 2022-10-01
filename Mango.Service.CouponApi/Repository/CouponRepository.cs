using AutoMapper;
using Mango.Service.CouponApi.Model;
using Mango.Service.CouponApi.Model.Dto;
using Mango.Services.CouponApi.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.CouponApi.Repository
{
    public class CouponRepository : ICouponRepository
    {
        #region Constructors

        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CouponRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        #endregion

        public async Task<CouponDto> GetByCode(string couponCode)
        {
            Coupon coupon = await this.dbContext.Coupons.FirstOrDefaultAsync(i => i.CouponCode == couponCode);
            return this.mapper.Map<CouponDto>(coupon);
        }
    }
}
