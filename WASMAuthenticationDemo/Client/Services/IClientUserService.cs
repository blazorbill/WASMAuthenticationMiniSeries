using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Shared;

namespace WASMAuthenticationDemo.Client.Services
{
    public interface IClientUserService
    {
        event EventHandler<UserAuthenticatedArgs> UserAuthenticatedEvent;
        Task<LoginResult> LoginUser(Credentials user);
        Task LogoutUser();
        Task<LoginResult> RegisterUser(UserRegistration user);
        Task<UserDTO> GetUserInfo(string Id);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<WeatherForecast[]> GetForecasts();
    }
}
