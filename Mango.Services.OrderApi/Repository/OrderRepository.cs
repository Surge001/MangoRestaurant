using Mango.Services.OrderApi.DbContexts;
using Mango.Services.OrderApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderApi.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using ApplicationDbContext db = new ApplicationDbContext(this.dbContext);
            db.OrderHeaders.Add(orderHeader);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using ApplicationDbContext db = new ApplicationDbContext(this.dbContext);
            var header = await db.OrderHeaders.FirstOrDefaultAsync(i => i.OrderHeaderId == orderHeaderId);

            if (header != null)
            {
                header.PaymentStatus = paid;
                await db.SaveChangesAsync();
            }
        }
    }
}
