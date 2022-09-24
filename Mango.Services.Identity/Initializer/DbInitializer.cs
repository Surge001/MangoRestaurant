using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        #region Private Fields

        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        #endregion

        #region Constructor

        public DbInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        #endregion

        public void Initialize()
        {
            if (this.roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                this.roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                this.roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else
                return; //<= If Admin role is found, it means first initial db setup already applied and no reason to run again;

            // Next below will create Admin user only once when identity tables are created on new database:
            ApplicationUser user = new ApplicationUser()
            {
                UserName = "admin1@gmail.com",
                Email = "admin1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "System",
                LastName = "Admin"
            };

            this.userManager.CreateAsync(user, "Admin123*").GetAwaiter().GetResult();
            this.userManager.AddToRoleAsync(user, SD.Admin).GetAwaiter().GetResult();
            var temp = this.userManager.AddClaimsAsync(user, new Claim[] { 
                new Claim(JwtClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtClaimTypes.Role, SD.Admin)
            }).Result;

            // Customer user creation when identity tables are created in the database:

            ApplicationUser user2 = new ApplicationUser()
            {
                UserName = "customer1@gmail.com",
                Email = "customer1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "2222222222",
                FirstName = "Regular",
                LastName = "User"
            };

            this.userManager.CreateAsync(user2, "Customer123*").GetAwaiter().GetResult();
            this.userManager.AddToRoleAsync(user2, SD.Admin).GetAwaiter().GetResult();
            var temp2 = this.userManager.AddClaimsAsync(user2, new Claim[] {
                new Claim(JwtClaimTypes.Name, user2.FirstName + " " + user2.LastName),
                new Claim(JwtClaimTypes.GivenName, user2.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user2.LastName),
                new Claim(JwtClaimTypes.Role, SD.Customer)
            }).Result;

        }
    }
}
