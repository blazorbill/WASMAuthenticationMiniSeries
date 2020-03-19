using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Server.Models;
using WASMAuthenticationDemo.Shared;

namespace WASMAuthenticationDemo.Server.Services
{
    public interface IUserAuthService
    {
        Task<LoginResult> RegisterUserAsync(UserRegistration user);
        Task<LoginResult> LoginUserAsync(Credentials user);
        WASMUser GetUser(string Id);
        IEnumerable<WASMUser> GetUsers();
    }
}
