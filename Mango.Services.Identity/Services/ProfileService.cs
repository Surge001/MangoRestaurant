using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipal;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ProfileService(IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipal, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userClaimsPrincipal = userClaimsPrincipal;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string subject = context.Subject.GetSubjectId(); //<= Came from request
            ApplicationUser user = await this.userManager.FindByIdAsync(subject); //<= came from database
            ClaimsPrincipal principal = await this.userClaimsPrincipal.CreateAsync(user); //<= created from database

            List<Claim> claims = principal.Claims.ToList(); //<= Claims for user from database
            //claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList(); //<= database claims which match from request

            if (this.userManager.SupportsUserRole)
            {
                IList<string> roles = await this.userManager.GetRolesAsync(user); //<= user roles from database
                foreach(var role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role)); //<= creates new claim for a role from database
                    if (this.roleManager.SupportsRoleClaims)
                    {
                        IdentityRole roleItem = await this.roleManager.FindByNameAsync(role); //<= Gets identity role from database
                        if(roleItem != null)
                        {
                            claims.AddRange(await this.roleManager.GetClaimsAsync(roleItem)); //<= Adds all claims for each found in DB roles
                        }
                    }
                }
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string subject = context.Subject.GetSubjectId();
            ApplicationUser user = await this.userManager.FindByIdAsync(subject);
            context.IsActive = user != null;

        }
    }
}
