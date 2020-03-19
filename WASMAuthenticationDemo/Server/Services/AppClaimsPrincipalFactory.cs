using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Server.Models;

namespace WASMAuthenticationDemo.Server.Services
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<WASMUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<WASMUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(WASMUser user)
        {
            var principal = await base.CreateAsync(user);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
        new Claim(ClaimTypes.GivenName, user.FirstName)
    });
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
         new Claim(ClaimTypes.Surname, user.LastName),
    });
            }

            return principal;
        }
    }
}
