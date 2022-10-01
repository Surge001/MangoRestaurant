
using Mango.Service.CouponApi.Model;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">Options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets Products
        /// </summary>
        public DbSet<Coupon> Coupons { get; set; }
        //public DbSet<CartHeader> CartHeaders { get; set; }
        //public DbSet<CartDetails> CartDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon()
                {
                    CouponId = 1,
                    CouponCode = "10OFF",
                    DiscountAmount = 10
                });
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon()
                {
                    CouponId = 2,
                    CouponCode = "20OFF",
                    DiscountAmount = 20
                });
        }
    }
}
