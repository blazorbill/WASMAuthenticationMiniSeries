using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Server.Services;
using WASMAuthenticationDemo.Shared;

namespace WASMAuthenticationDemo.Server.Models
{
    public class WASMAuthenticationDemoContext : IdentityDbContext<WASMUser>
    {
        public WASMAuthenticationDemoContext()
        {
        }
        public WASMAuthenticationDemoContext(DbContextOptions<WASMAuthenticationDemoContext> options)
                : base(options)
        {
        }

    }
}
