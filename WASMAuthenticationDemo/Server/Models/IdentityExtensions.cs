using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;


namespace WASMAuthenticationDemo.Server.Models
{
    public static class IdentityExtensions
    {
        public static string FirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.GivenName);
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }


        public static string LastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Surname);
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }


    }
}
