using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> dbContext;

        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            // ToDo: implement email sender with any library of your choosing;

            EmailLog logEntry = new()
            {
                Email = message.Email,
                SentUtc = DateTime.UtcNow,
                Log = $"Order - {message.OrderId} has been created successfully."
            };

            ApplicationDbContext db = new ApplicationDbContext(this.dbContext);
            db.EmailLogs.Add(logEntry);
            await db.SaveChangesAsync();
        }
    }
}
