using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Shared;

namespace WASMAuthenticationDemo.Client.Services
{
    public interface IWASMAuthenticationStateProvider
    {
        Task MarkUserAsAuthenticated(LoginResult loginResult);
        Task MarkUserAsLoggedOut();
    }
}
